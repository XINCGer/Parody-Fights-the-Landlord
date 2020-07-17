//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework.Foundation.DownLoad
{

    public class TimeoutAsyncOperation : IEnumerator
    {
        private float _beginTime;
        private float _timeoutS;

        public AsyncOperation AsyncOperation;
        public bool IsTimeout { get; private set; }

        public TimeoutAsyncOperation(AsyncOperation asyncOpr, float timeoutS)
        {
            AsyncOperation = asyncOpr;
            _timeoutS = timeoutS;
            _beginTime = Time.unscaledTime;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {

        }

        public bool IsDone()
        {
            if (AsyncOperation.isDone)
            {
                IsTimeout = false;
                return true;
            }
            else
            {
                float now = Time.unscaledTime;
                if (now - _beginTime >= _timeoutS)
                {
                    IsTimeout = true;
                    return true;
                }
                return false;
            }
        }
    }
}