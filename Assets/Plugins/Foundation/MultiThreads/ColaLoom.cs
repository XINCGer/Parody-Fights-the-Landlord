//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ColaFramework.Foundation
{
    /// <summary>
    /// ColaLoom 多线程工具类，支持在Unity多线程操作
    /// </summary>
    public class ColaLoom : MonoBehaviour
    {

        /// <summary>
        /// 支持开启的最大线程数
        /// </summary>
        public static int maxThreads = 8;
        private static int numThreads;
        private static ColaLoom _current;
        private int _count;
        private List<Action> _currentActions = new List<Action>();

        public static ColaLoom Current
        {
            get
            {
                Initialize();
                return _current;
            }
        }
        private static bool initialized;
        private List<Action> _actions = new List<Action>();
        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }
        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();


        void Awake()
        {
            _current = this;
            initialized = true;
        }

        /// <summary>
        /// 初始化ColaLoom多线程工具，只在游戏开始调用一次就好
        /// </summary>
        public static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("ColaLoom");
                DontDestroyOnLoad(g);
                _current = g.AddComponent<ColaLoom>();
            }
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (Current._delayed)
                {
                    Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (Current._actions)
                {
                    Current._actions.Add(action);
                }
            }
        }

        public static Thread RunAsync(Action a)
        {
            Initialize();
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }

        }


        void OnDisable()
        {
            if (_current == this)
            {
                _current = null;
            }
        }


        // Update is called once per frame
        void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (var a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                for (int i = 0; i < _delayed.Count; i++)
                {
                    if (_delayed[i].time <= Time.time)
                    {
                        _currentDelayed.Add(_delayed[i]);
                    }
                }
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }
        }
    }
}