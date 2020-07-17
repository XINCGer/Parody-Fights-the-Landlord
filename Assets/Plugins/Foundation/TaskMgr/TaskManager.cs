using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ColaFramework.Foundation
{
    /// <summary>
    /// 底层基于携程封装的任务调度管理器
    /// </summary>
    public class TaskManager
    {
        #region Instance
        private static TaskManager s_instance = null;
        public static TaskManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new TaskManager();
                }
                return s_instance;
            }
        }
        #endregion

        public void Init(MonoBehaviour behaviour)
        {
            Task.Init(behaviour);
        }

        private int m_seq = 1;
        private Dictionary<int, Task> m_dicTask = new Dictionary<int, Task>();
        //创建一个任务 返回任务ID
        public int Create(IEnumerator c, bool autoStart = true)
        {
            int taskID = m_seq++;
            Task task = new Task(taskID, c);
            if (autoStart)
            {
                task.Start();
            }

            m_dicTask.Add(taskID, task);
            return taskID;
        }
        //停止任务
        public void StopTask(int taskID)
        {
            Task task = null;
            m_dicTask.TryGetValue(taskID, out task);
            if (task != null)
            {
                task.Stop();
            }
        }
        public void RemoveTask(int taskID)  //外部禁止调用，由Task调用
        {
            if (m_dicTask.ContainsKey(taskID))
            {
                m_dicTask.Remove(taskID);
            }
        }
    }

    public class Task
    {

        private static MonoBehaviour s_behaviour = null;
        public static void Init(MonoBehaviour behaviour)
        {
            s_behaviour = behaviour;
        }


        private IEnumerator m_coroutine;
        private bool m_bRunning;
        public bool Running { get { return m_bRunning; } }
        private bool m_bPaused;
        public bool Paused { get { return m_bPaused; } }
        private int m_taskID;

        public Task(int taskID, IEnumerator c)
        {
            if (s_behaviour == null)
            {
                Debug.LogError("Task must be init before use!!");
            }
            m_taskID = taskID;
            m_coroutine = c;
        }

        public void Start()
        {
            m_bRunning = true;
            s_behaviour.StartCoroutine(CallWrapper());
        }

        public void Stop()
        {
            m_bRunning = false;
        }

        public void Pause()
        {
            m_bPaused = true;
        }

        public void Unpause()
        {
            m_bPaused = false;
        }

        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = m_coroutine;
            while (m_bRunning)
            {
                if (m_bPaused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        m_bRunning = false;
                    }
                }
            }
            TaskManager.Instance.RemoveTask(m_taskID);
        }
    }
}