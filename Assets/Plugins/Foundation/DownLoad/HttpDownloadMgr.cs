//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace ColaFramework.Foundation.DownLoad
{

    /// <summary>
    /// 底层使用UnityWebRequest的下载工具类
    /// </summary>
    public class HttpDownloadMgr
    {
        public static void DownloadText(string url, CompletedTextHandler onComplete, float timeout = 10)
        {
            TaskManager.Instance.Create(StartWebGet(url, onComplete, timeout));
        }

        static IEnumerator StartWebGet(string url, CompletedTextHandler onComplete, float timeout = 10)
        {

            UnityWebRequest request = UnityWebRequest.Get(url);
            // 设置10s超时
            TimeoutAsyncOperation asynOpr = new TimeoutAsyncOperation(request.Send(), timeout);

            yield return asynOpr;

            if (asynOpr.IsTimeout)
            {
                // Debug.Log("WebGet,超时，尝试重新获取，超时时间：{0},url:{1}",timeout, url);
                // TaskManager.Instance.Create(StartWebGet(url, onComplete,timeout));
                onComplete(ErrorCode.TIME_OUT, string.Format("WebGet, 超时，尝试重新获取，超时时间：{0},url: {1}", timeout, url), "");
            }
            else
            {
                if (request.responseCode == 200)
                {
                    string text = request.downloadHandler.text;
                    onComplete(ErrorCode.SUCCESS, null, text);
                }
                else
                {
                    Debug.LogWarningFormat("StartWebGet, Status Code: {0}", request.responseCode);
                    onComplete(ErrorCode.ERROR, null, request.responseCode.ToString());
                }
            }
        }
    }
}