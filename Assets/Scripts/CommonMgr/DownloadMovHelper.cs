//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 下载视频的工具类
/// </summary>
public class DownloadMovHelper
{
    /// <summary>
    /// 是否正在下载的标志位
    /// </summary>
    private static bool isLoading = false;

    /// <summary>
    /// 本地文件的路径
    /// </summary>
    private static string localFilePath;

    /// <summary>
    /// 下载的网络资源路径
    /// </summary>
    private static string downloadURL;

    private static Action _onLoading;
    private static Action _onCompleted;
    private static Action<DownLoadMovError> _onFailed;
    private static Action<int> _onProgress;

    private static WaitForSeconds oneSecond = new WaitForSeconds(0.5f);

    private static WWW www;
    private static UnityWebRequest webRequest;

    /// <summary>
    /// 对外提供的开始下载接口
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="url"></param>
    /// <param name="onLoading"></param>
    /// <param name="onComplete"></param>
    /// <param name="onFailed"></param>
    /// <param name="onProgress"></param>
    public static void Begin(string filePath, string url, Action onLoading, Action onComplete, Action<DownLoadMovError> onFailed, Action<int> onProgress)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("视频本地路径或者网络资源路径为空！");
            if (null != onFailed)
            {
                onFailed(DownLoadMovError.PathError);
            }
            return;
        }

        _onLoading = onLoading;
        _onCompleted = onComplete;
        _onFailed = onFailed;
        _onProgress = onProgress;
        localFilePath = filePath;
        downloadURL = url;

        if (CheckLocalFileExist(filePath))
        {
            if (null != onComplete)
            {
                onComplete();
            }
        }
        else
        {
            isLoading = true;
            if (null != _onLoading)
            {
                onLoading();
            }
            //GameLauncher.Instance.StartCoroutine(DownloadObsolete(downloadURL));
            GameLauncher.Instance.StartCoroutine(Download(downloadURL));
        }
    }

    /// <summary>
    /// 停止下载
    /// </summary>
    public static void Stop()
    {
        if (isLoading)
        {
            if (null != webRequest)
            {
                webRequest.Abort();
            }
            GameLauncher.Instance.StopCoroutine(Download(downloadURL));
        }
        Release();
    }

    /// <summary>
    /// 释放清理之前的回调和下载进程，本地缓存不会被清理掉
    /// </summary>
    public static void Release()
    {
        isLoading = false;
        localFilePath = string.Empty;
        downloadURL = string.Empty;
        _onLoading = null;
        _onCompleted = null;
        _onFailed = null;
        _onProgress = null;
    }

    /// <summary>
    /// 删除本地的视频资源缓存
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="onComplete"></param>
    /// <param name="onFailed"></param>
    public static void DeleteVideo(string filePath, Action onComplete, Action onFailed)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            if (null != onFailed)
            {
                onFailed();
            }
            return;
        }

        if (CheckLocalFileExist(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                if (null != onFailed)
                {
                    onFailed();
                }
                return;
            }

            if (null != onComplete)
            {
                onComplete();
            }
        }
        else
        {
            if (null != onFailed)
            {
                onFailed();
            }
        }
    }

    /// <summary>
    /// 执行下载视频任务的协程（旧版本）
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Obsolete("该接口已经过时")]
    private static IEnumerator DownloadObsolete(string url)
    {
        if (string.IsNullOrEmpty(url)) yield break;
        www = new WWW(url);
        int progress = 0;
        while (!www.isDone)
        {
            progress = (int)(www.progress * 100) % 100;
            if (null != _onProgress)
            {
                _onProgress(progress);
            }

            yield return oneSecond;
        }

        if (www.isDone)
        {
            if (www.bytes.Length > 0 && !string.IsNullOrEmpty(localFilePath))
            {
                byte[] btArray = www.bytes;
                try
                {
                    FileStream WriteStream = new FileStream(localFilePath, FileMode.Create);
                    WriteStream.Write(btArray, 0, btArray.Length);
                    WriteStream.Close();
                    WriteStream.Dispose();
                }
                catch (Exception e)
                {
                    if (null != _onFailed)
                    {
                        _onFailed(DownLoadMovError.SaveError);
                    }
                    throw;
                }
                isLoading = false;

                //双重检查
                if (CheckLocalFileExist(localFilePath))
                {
                    if (null != localFilePath)
                    {
                        _onCompleted();
                    }
                }
                else
                {
                    if (null != _onFailed)
                    {
                        _onFailed(DownLoadMovError.SaveError);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(www.error) || www.bytes.Length <= 0)
        {
            isLoading = false;
            if (null != _onFailed)
            {
                _onFailed(DownLoadMovError.DownloadError);
            }
        }
        www.Dispose();
        www = null;
    }

    /// <summary>
    /// 执行下载视频任务的协程
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static IEnumerator Download(string url)
    {
        if (string.IsNullOrEmpty(url)) yield break;
        webRequest = UnityWebRequest.Get(url);
        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        int progress = 0;
        while (!asyncOperation.isDone)
        {
            progress = (int)(asyncOperation.progress * 100) % 100;
            if (null != _onProgress)
            {
                _onProgress(progress);
            }

            yield return oneSecond;
        }

        if (asyncOperation.isDone)
        {
            if (webRequest.downloadedBytes > 0 && !string.IsNullOrEmpty(localFilePath))
            {
                byte[] btArray = webRequest.downloadHandler.data;
                try
                {
                    FileStream WriteStream = new FileStream(localFilePath, FileMode.Create);
                    WriteStream.Write(btArray, 0, btArray.Length);
                    WriteStream.Close();
                    WriteStream.Dispose();
                }
                catch (Exception e)
                {
                    if (null != _onFailed)
                    {
                        _onFailed(DownLoadMovError.SaveError);
                    }
                    throw;
                }
                isLoading = false;

                //双重检查
                if (CheckLocalFileExist(localFilePath))
                {
                    if (null != localFilePath)
                    {
                        _onCompleted();
                    }
                }
                else
                {
                    if (null != _onFailed)
                    {
                        _onFailed(DownLoadMovError.SaveError);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(webRequest.error) || webRequest.downloadedBytes <= 0)
        {
            isLoading = false;
            if (null != _onFailed)
            {
                _onFailed(DownLoadMovError.DownloadError);
            }
        }
        webRequest.Dispose();
        webRequest = null;
    }

    /// <summary>
    /// 检查本地文件是否存在,如果目录不存在则创建目录
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckLocalFileExist(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }
        string dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            return false;
        }

        if (File.Exists(filePath))
        {
            return true;
        }
        return false;
    }
}

/// <summary>
/// 下载视频错误的枚举码
/// </summary>
public enum DownLoadMovError : byte
{
    /// <summary>
    /// 路径错误
    /// </summary>
    PathError,

    /// <summary>
    /// 保存失败
    /// </summary>
    SaveError,

    /// <summary>
    /// 资源下载失败
    /// </summary>
    DownloadError,
}