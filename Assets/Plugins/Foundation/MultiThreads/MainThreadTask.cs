//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Threading;
using UnityEngine;

namespace ColaFramework.Foundation
{
    /// <summary>
    /// 工具类，让子线程可以在主线程中调用代码
    /// </summary>
    public class MainThreadTask : MonoBehaviour
    {
        /// <summary>
        /// 必须在主线程中调用
        /// </summary>
        public static void Init()
        {
            m_mainThreadID = Thread.CurrentThread.ManagedThreadId;
            var obj = new GameObject("MainThreadTask");
            obj.AddComponent<MainThreadTask>();
        }

        /// <summary>
        /// 异步调用函数
        /// </summary>
        /// <param name="action">欲调用的函数</param>
        public static void RunAsync(Action action)
        {
            RunAsync(MakeCoroutineFromAction(action));
        }

        /// <summary>
        /// 异步调用协程
        /// </summary>
        /// <param name="coroutine">欲调用的协程</param>
        public static void RunAsync(IEnumerator coroutine)
        {
            lock (m_taskQueue)
                m_taskQueue.Enqueue(coroutine);
        }

        /// <summary>
        /// 异步调用函数并等待其完成 (有死锁风险)
        /// </summary>
        /// <param name="action">欲调用的函数</param>
        public static void RunUntilFinish(Action action, Flag cancelFlag = null)
        {
            if (IsMainThread())
                action();
            else
                RunUntilFinish(MakeCoroutineFromAction(action), cancelFlag);
        }

        /// <summary>
        /// 异步调用协程并等待其完成 (有死锁风险)
        /// </summary>
        /// <param name="coroutine">欲调用的协程</param>
        public static void RunUntilFinish(IEnumerator coroutine, Flag cancelFlag = null)
        {
            if (IsMainThread())
                throw new Exception("can not call this method in main thread, will cause deadlock");

            Flag finishFlag = new Flag();
            lock (m_taskQueue)
                m_taskQueue.Enqueue(WrapCoroutineAndSetFinishFlag(coroutine, finishFlag));

            while ((cancelFlag == null || !cancelFlag.IsSet()) && !finishFlag.IsSet() && !m_stopped)
            {
                Thread.Sleep(20);
            }
        }

        private static Int32 m_mainThreadID;
        private static Queue<IEnumerator> m_taskQueue = new Queue<IEnumerator>();
        private static volatile Boolean m_stopped = false;

        private static Boolean IsMainThread()
        {
            //return Thread.CurrentThread.ManagedThreadId == m_mainThreadID;	//Thread.CurrentThread.ManagedThreadId incorrect !?
            return false;
        }

        private static IEnumerator MakeCoroutineFromAction(Action action)
        {
            action();
            yield break;
        }

        public class Flag
        {
            /// <summary>
            /// non-zero means true. VolatileXXX restrict
            /// </summary>
            private Int32 m_isFinished = 0;

            public Boolean IsSet()
            {
                return Thread.VolatileRead(ref m_isFinished) != 0;
            }

            public void Set()
            {
                Thread.VolatileWrite(ref m_isFinished, 1);
            }

            //public void Clear()
            //{
            //    Thread.VolatileWrite(ref m_isFinished, 0);
            //}
        }

        private static IEnumerator WrapCoroutineAndSetFinishFlag(IEnumerator coroutine, Flag finishFlag)
        {
            try
            {
                while (coroutine.MoveNext())
                    yield return coroutine.Current;
            }
            finally
            {
                finishFlag.Set();
            }
        }

        private void Update()
        {
            //start all queued task
            while (true)
            {
                Boolean hasMore = false;
                IEnumerator coroutine = null;
                lock (m_taskQueue)
                {
                    if (m_taskQueue.Count > 0)
                    {
                        hasMore = true;
                        coroutine = m_taskQueue.Dequeue();
                    }
                }

                if (hasMore)
                {
                    StartCoroutine(coroutine);
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        private void OnEnable()
        {
            m_stopped = false;
        }

        private void OnDisable()
        {
            m_stopped = true;
        }
    }
}