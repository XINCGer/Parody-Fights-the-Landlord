//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;

namespace ColaFramework.Foundation.DownLoad
{
    public delegate void ProgressHandler(float progress, int index, int count, int diff);           // 进度回调
    public delegate void CompletedHandler(ErrorCode code, string msg, byte[] bytes);    // 完成回调
    public delegate void CompletedTextHandler(ErrorCode code, string msg, string text); // 完成回调
    public delegate void LaunchConfirmHandler(string strDesc, string strBtn, System.Action onConfirm, bool contactGM);
    public delegate void UpdateNoticeHandler(string strTitle, string strDesc, System.Action onConfirm, System.Action onSkip);

    public enum ErrorCode
    {
        SUCCESS,
        ERROR,
        TIME_OUT,
    }

    public class Downloader : WorkBase
    {
        private ProgressHandler m_onProgress;
        private CompletedHandler m_onCompleted;

        UnityWebRequest m_request = null;
        DownloaderHandler m_downloadHandler = null;
        string m_strUrl;
        string m_strPath;

        public Downloader(string url, string path, ProgressHandler onProgress = null, CompletedHandler onComplete = null)
        {
            m_onProgress = onProgress;
            m_onCompleted = onComplete;

            m_strUrl = url;
            m_strPath = path;
            IsFinish = false;
        }

        public override void Dispose()
        {
            m_onProgress = null;
            m_onCompleted = null;
            if (m_request != null)
            {
                if (!m_request.isDone)
                {
                    m_request.Abort();
                }

                m_request.Dispose();
                m_request = null;
            }

            if (m_downloadHandler != null)
            {
                m_downloadHandler.Close();
                m_downloadHandler = null;
            }
        }

        public override void Run()
        {
            m_request = UnityWebRequest.Get(m_strUrl);
            if (!string.IsNullOrEmpty(m_strPath))
            {
                // 是写到文件的情况开启断点续传
                m_downloadHandler = new DownloaderHandler(m_strPath, OnProgress, OnCompleted);

                m_request.chunkedTransfer = true;
                m_request.disposeDownloadHandlerOnDispose = true;
                m_request.SetRequestHeader("Range", "bytes=" + m_downloadHandler.DownedLength + "-");
                m_request.downloadHandler = m_downloadHandler;
            }

            m_request.Send();
            Debug.LogFormat("Downloader: {0} => {1}", m_strUrl, m_strPath);
        }

        public override void Update()
        {
            if (m_request == null)
            {
                return;
            }
            if (IsFinish)
            {
                return;
            }
            if (!m_request.isDone)
            {
                if (m_downloadHandler == null)
                {
                    OnProgress(m_request.downloadProgress, 0, 1, 0);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(m_request.error))
                {
                    // 错误
                    OnCompleted(ErrorCode.ERROR, m_request.error);
                }
                else
                {
                    long status = 0;
                    status = m_request.responseCode;
                    Debug.LogWarning("在线文件下载失败，错误码：" + status);
                    switch (status)
                    {
                        case 200:
                            // 正常正确返回码是200
                            OnCompleted(ErrorCode.SUCCESS, "", m_request.downloadHandler.data);
                            break;

                        case 206:
                            // 断点续传正确返回码是206
                            // 这种情况DownloaderHandler在完成时会回调,所以这边不需要处理
                            break;

                        default:
                            // 其它:错误
                            OnCompleted(ErrorCode.ERROR, string.Format("ResponseCode Failed: {0}", status));
                            break;
                    }
                }
            }
        }

        void OnProgress(float progress, int index, int count, int diff)
        {
            if (m_onProgress != null)
            {
                m_onProgress(progress, index, count, diff);
            }
        }

        void OnCompleted(ErrorCode code, string error = "", byte[] bytes = null)
        {
            if (m_onCompleted != null)
            {
                m_onCompleted(code, error, bytes);
            }

            IsFinish = true;
        }
    }

    public class DownloaderHandler : DownloadHandlerScript
    {
        private string m_path;
        private string m_pathTemp;
        private FileStream m_stream;

        //要下载的文件总长度
        private int m_totalLength = 0;
        private int m_contentLength = 0;
        private int m_downedLength = 0;

        CompletedHandler m_onCompleted;
        ProgressHandler m_onProgress;

        public DownloaderHandler(string path, ProgressHandler onProgress, CompletedHandler onCompleted)
            : base(new byte[1024 * 200])
        {
            m_onCompleted = onCompleted;
            m_onProgress = onProgress;

            m_path = path.Replace('\\', '/');

            // 临时缓存文件，结尾添加.temp扩展名
            m_pathTemp = m_path + ".temp";
            FileHelper.EnsureParentDirExist(m_pathTemp);
            m_stream = new FileStream(m_pathTemp, FileMode.OpenOrCreate);

            m_downedLength = (int)m_stream.Length;
            m_stream.Position = m_downedLength;
        }

        // 已下载的长度
        public int DownedLength
        {
            get
            {
                return m_downedLength;
            }
        }

        public void Close()
        {
            if (m_stream != null)
            {
                m_stream.Close();
                m_stream.Dispose();

                m_stream = null;
            }
        }
        void OnCompleted(ErrorCode code, string error = "", byte[] bytes = null)
        {
            if (m_onCompleted != null)
            {
                m_onCompleted(code, error, bytes);
            }
        }

        // 下载完成
        protected override void CompleteContent()
        {
            Close();

            if (m_contentLength == 0)
            {
                // 没有下载到数据
                OnCompleted(ErrorCode.ERROR, "Content length is zero");
            }
            else
            {
                if (!File.Exists(m_pathTemp))
                {
                    // 没有找到缓存文件
                    OnCompleted(ErrorCode.ERROR, "Temp file miss");
                }
                else
                {
                    if (File.Exists(m_path))
                    {
                        File.Delete(m_path);
                    }
                    File.Move(m_pathTemp, m_path);

                    // 成功
                    OnCompleted(ErrorCode.SUCCESS);
                }
            }
        }

        // 接收到要下载的长度
        protected override void ReceiveContentLength(int contentLength)
        {
            Debug.LogFormat("已下载：{0}, ReceiveContentLength {1}", m_downedLength, contentLength);

            m_contentLength = contentLength;
            m_totalLength = contentLength + m_downedLength;
        }

        // 接收到数据
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (m_contentLength == 0)
            {
                return false;
            }

            if (data == null || data.Length == 0)
            {
                return false;
            }

            m_stream.Write(data, 0, dataLength);
            m_downedLength += dataLength;

            if (m_onProgress != null)
            {
                m_onProgress((float)m_downedLength / m_totalLength, m_downedLength, m_totalLength, dataLength);
            }

            return true;
        }
    }
}