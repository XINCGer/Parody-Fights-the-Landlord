//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace ColaFramework.Foundation.DownLoad
{
	public enum DownloadTaskStatus
	{
		Paused = 0,
		Waiting,		//waiting in queue
		Downloading,	//active downloading
		Finished,
		Failed,
	}

	public static class DownloadTaskStatusExtension
	{
		public static Boolean CanStart(this DownloadTaskStatus status)
		{
			return status == DownloadTaskStatus.Paused || status == DownloadTaskStatus.Failed;
		}

		public static Boolean CanPause(this DownloadTaskStatus status)
		{
			return status == DownloadTaskStatus.Waiting || status == DownloadTaskStatus.Downloading;
		}

		public static Boolean HasError(this DownloadTaskStatus status)
		{
			return status == DownloadTaskStatus.Failed;
		}
	}

	public enum DownloadTaskErrorCode
	{
		Success = 0,
		NetworkError,
		IOError,
		Md5Dismatch,
	}

	/// <summary>
	/// task info of DownloadMgr
	/// </summary>
	public struct DownloadTaskInfo
	{
		public DownloadTaskInfo(String md5, String url, String localPath, DownloadTaskStatus status,
			Int64 totalSize, Int64 finishedSize)
		{
			this.md5 = md5;
			this.url = url;
			this.localPath = localPath;
			this.status = status;
			this.totalSize = totalSize;
			this.finishedSize = finishedSize;
		}

		public readonly String md5;
		public readonly String url;
		public readonly String localPath;
		public readonly DownloadTaskStatus status;
		public readonly Int64 totalSize;
		public readonly Int64 finishedSize;
	}

	public struct DownloadTaskErrorInfo
	{
		public readonly DownloadTaskErrorCode errorCode;
		public Int32 innerErrorCode;
		public readonly String errorMessage;

		public DownloadTaskErrorInfo(DownloadTaskErrorCode errorCode, Int32 innerErrorCode, String errorMessage)
		{
			this.errorCode = errorCode;
			this.innerErrorCode = innerErrorCode;
			this.errorMessage = errorMessage;
		}
	}

	/// <summary>
	/// A file download manager to manage downloading state of serveral task
	/// </summary>
	public class DownloadMgr : IDisposable
	{
		public DownloadMgr(String rootDir)
		{
			m_rootDir = rootDir;
		}

		void IDisposable.Dispose()
		{
			lock (m_lock)
			{
				foreach (var item in m_taskMap)
				{
					m_taskQueue.Clear();

					Task task = item.Value;
					if (task.status.CanPause())
						task.status = DownloadTaskStatus.Paused;
				}
				m_taskMap.Clear();
			}
		}

		String m_rootDir;

		/// <summary>
		/// add new task. the status of the new task will be "Paused"
		/// if task with the same md5 exists, remove it before add new task
		/// </summary>
		/// <param name="md5"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public DownloadTaskInfo AddTask(String md5, String url)
		{
			lock (m_lock)
			{
				{
					Task existTask;
					if (m_taskMap.TryGetValue(md5, out existTask))
						return existTask.ToDownloadTaskInfo();
				}

				Task task = new Task();
				task.md5 = md5;
				task.url = url;
				task.localPath = Path.Combine(m_rootDir, md5);
				task.status = DownloadTaskStatus.Paused;
				task.totalSize = 0;		//unknown
				task.finishedSize = 0;
				task.errorCode = DownloadTaskErrorCode.Success;

				m_taskMap.Add(md5, task);
				return task.ToDownloadTaskInfo();
			}
		}

		/// <summary>
		/// start task with the given md5
		/// </summary>
		/// <param name="md5"></param>
		/// <returns>if one task started, return true</returns>
		public Boolean StartTask(String md5)
		{
			lock (m_lock)
			{
				Task existTask;
				if (!m_taskMap.TryGetValue(md5, out existTask))
					return false;		//task not exist

				if (!existTask.status.CanStart())
					return false;

				if (!m_taskQueue.Contains(existTask))
					m_taskQueue.Add(existTask);

				existTask.status = DownloadTaskStatus.Waiting;
			}

			UpdateActiveTask();
			return true;
		}

		/// <summary>
		/// pause task with the given md5
		/// </summary>
		/// <param name="md5"></param>
		/// <returns>if one task paused, return true</returns>
		public Boolean PauseTask(String md5)
		{
			lock (m_lock)
			{
				Task task;
				if (!m_taskMap.TryGetValue(md5, out task))
					return false;

				if (!task.status.CanPause())
					return false;

				task.status = DownloadTaskStatus.Paused;
				m_taskQueue.Remove(task);
			}

			UpdateActiveTask();
			return true;
		}

		/// <summary>
		/// remove task with the given md5. local file is also removed
		/// </summary>
		/// <param name="md5"></param>
		/// <returns>if one task removed, return true</returns>
		public Boolean RemoveTask(String md5)
		{
			Boolean bValid = false;
			lock (m_lock)
			{
				Task existTask;
				if (m_taskMap.TryGetValue(md5, out existTask))
				{
					//if downloading, paused it first
					if (existTask.status.CanPause())
						existTask.status = DownloadTaskStatus.Paused;

					bValid = m_taskMap.Remove(md5);
					m_taskQueue.Remove(existTask);
				}
			}

			if (bValid)
				UpdateActiveTask();
			return bValid;
		}

		/// <summary>
		/// find task by md5
		/// </summary>
		/// <param name="md5"></param>
		/// <param name="info">receive task info</param>
		/// <returns>return true when found</returns>
		public Boolean FindTask(String md5, out DownloadTaskInfo info)
		{
			lock (m_lock)
			{
				Task task;
				if (m_taskMap.TryGetValue(md5, out task))
				{
					info = task.ToDownloadTaskInfo();
					return true;
				}
			}

			info = new DownloadTaskInfo();
			return false;
		}

		/// <summary>
		/// valid when FindTask get task with status as Failed
		/// </summary>
		/// <param name="md5"></param>
		/// <param name="errorInfo"></param>
		/// <returns></returns>
		public Boolean GetTaskErrorInfo(String md5, out DownloadTaskErrorInfo errorInfo)
		{
			lock (m_lock)
			{
				Task task;
				if (m_taskMap.TryGetValue(md5, out task))
				{
					if (task.status.HasError())
					{
						errorInfo = task.ToDownloadTaskErrorInfo();
						return true;
					}
				}
			}

			errorInfo = new DownloadTaskErrorInfo();
			return false;
		}

		public IEnumerable<DownloadTaskInfo> Tasks
		{
			get
			{
				foreach (var vpair in m_taskMap)
				{
					yield return vpair.Value.ToDownloadTaskInfo();
				}
			}
		}

		public class TaskEndArgs : EventArgs
		{
			public DownloadTaskInfo TaskInfo { get { return m_taskInfo; } }
			private DownloadTaskInfo m_taskInfo;
			public TaskEndArgs(DownloadTaskInfo taskInfo)
			{
				m_taskInfo = taskInfo;
			}
		}
		public event EventHandler<TaskEndArgs> TaskEndEvent;

		protected void RaiseTaskEndEvent(DownloadTaskInfo taskInfo)
		{
			EventHandler<TaskEndArgs> temp = Interlocked.CompareExchange(ref TaskEndEvent, null, null);
			if (temp != null)
				temp(this, new TaskEndArgs(taskInfo));
		}

		private class Task
		{
			public String md5;
			public String url;
			public String localPath;
			public DownloadTaskStatus status;
			public Int64 totalSize;
			public Int64 finishedSize;

			public DownloadTaskErrorCode errorCode;
			public Int32 innerErrorCode;
			public String errorMessage;

			public DownloadTaskInfo ToDownloadTaskInfo()
			{
				return new DownloadTaskInfo(md5, url, localPath, status, totalSize, finishedSize);
			}

			public DownloadTaskErrorInfo ToDownloadTaskErrorInfo()
			{
				return new DownloadTaskErrorInfo(errorCode, innerErrorCode, errorMessage);
			}
		}

		Object m_lock = new Object();
		Dictionary<String, Task> m_taskMap = new Dictionary<String, Task>();
		List<Task> m_taskQueue = new List<Task>();

		private Thread m_worker;

		/// <summary>
		/// start m_worker if need
		/// </summary>
		private void UpdateActiveTask()
		{
			lock (m_lock)
			{
				if (m_worker != null)
					return;		//m_worker is already running

				if (m_taskQueue.Count == 0)		//nothing to do
					return;
			}

			StartWorker();
		}

		private void StartWorker()
		{
			lock (m_lock)
			{
				m_worker = new Thread(() => WorkProc());
				m_worker.Start();
			}
		}

		private static String CalcFileMd5AndSize(String filePath, out Int64 fileSize)
		{
			try
			{
				var md5 = MD5.Create();
				using (var stream = File.OpenRead(filePath))
				{
					fileSize = stream.Length;
					Byte[] data = md5.ComputeHash(stream);
					return BitConverter.ToString(data).Replace("-","").ToLower();
				}
			}
			catch (IOException)
			{
				fileSize = 0;
				return "";
			}
		}

		private static String CalcFileMd5(String filePath)
		{
			Int64 fileSize;
			return CalcFileMd5AndSize(filePath, out fileSize);
		}

		private void WorkProc()
		{
			while (WorkOnce())
			{}

			lock (m_lock)
			{
				m_worker = null;
			}
		}

		/// <summary>
		/// return false when no more task
		/// </summary>
		/// <returns></returns>
		private Boolean WorkOnce()
		{
			Task activeTask;
			DownloadTaskInfo activeTaskSnapshot;

			lock (m_lock)
			{
				if (m_taskQueue.Count == 0)		//nothing to do
					return false;

				activeTask = m_taskQueue[0];
				m_taskQueue.RemoveAt(0);

				activeTaskSnapshot = activeTask.ToDownloadTaskInfo();

				activeTask.status = DownloadTaskStatus.Downloading;
			}

			if (File.Exists(activeTaskSnapshot.localPath))	//local file exist, check whether already finished
			{
				if (!FileDownloadHelper.IsFileInProgress(activeTaskSnapshot.localPath))
				{
					Int64 fileSize;
					if (CalcFileMd5AndSize(activeTaskSnapshot.localPath, out fileSize) == activeTaskSnapshot.md5)
					{
						lock (m_lock)
						{
							activeTask.status = DownloadTaskStatus.Finished;
							activeTask.totalSize = fileSize;
							activeTask.finishedSize = fileSize;
							RaiseTaskEndEvent(activeTask.ToDownloadTaskInfo());
							return true;
						}
					}
					else	//file content is not correct
					{
						File.Delete(activeTaskSnapshot.localPath);

						//download again
					}
				}
			}

			FileDownloadHelper downloadHelper = new FileDownloadHelper();

			Boolean bCanceled = false;

			downloadHelper.ProgressChanged += (sender, arg) =>
				{
					//update task status
					lock (m_lock)
					{
						if (!activeTask.status.CanPause())	//stopped
						{
							downloadHelper.Cancel();
							bCanceled = true;
						}
						else
						{
							activeTask.totalSize = arg.TotalFileSize;
							activeTask.finishedSize = arg.CurrentFileSize;
						}
					}
				};

			downloadHelper.DownloadComplete += (sender, arg) =>
				{
				};

			try
			{
				Directory.CreateDirectory(m_rootDir);

				downloadHelper.Download(activeTask.url, activeTask.localPath);
				if (!bCanceled)
				{
					Int64 fileSize;
					if (CalcFileMd5AndSize(activeTaskSnapshot.localPath, out fileSize) == activeTaskSnapshot.md5)
					{
						lock (m_lock)
						{
							activeTask.status = DownloadTaskStatus.Finished;
							activeTask.totalSize = fileSize;
							activeTask.finishedSize = fileSize;
						}
					}
					else	//md5 dismatch
					{
						File.Delete(activeTaskSnapshot.localPath);
						lock (m_lock)
						{
							activeTask.status = DownloadTaskStatus.Failed;
							activeTask.errorCode = DownloadTaskErrorCode.Md5Dismatch;
						}
					}
				}
			}
			catch (WebException e)
			{
				lock (m_lock)
				{
					activeTask.status = DownloadTaskStatus.Failed;
					activeTask.errorCode = DownloadTaskErrorCode.NetworkError;
					activeTask.innerErrorCode = (Int32)e.Status;
					activeTask.errorMessage = e.Message;
				}
			}
			catch (IOException e)
			{
				lock (m_lock)
				{
					activeTask.status = DownloadTaskStatus.Failed;
					activeTask.errorCode = DownloadTaskErrorCode.IOError;
					activeTask.innerErrorCode = 0;
					activeTask.errorMessage = e.Message;
				}
			}

			if (!bCanceled)
				RaiseTaskEndEvent(activeTask.ToDownloadTaskInfo());

			return true;
		}
	}
}
