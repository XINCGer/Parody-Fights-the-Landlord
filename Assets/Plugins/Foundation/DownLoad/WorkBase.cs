//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace ColaFramework.Foundation.DownLoad
{
    /// <summary>
    /// 下载任务队列的基类
    /// </summary>
    public class WorkBase
    {
        public virtual void Update() { }
        public virtual void Dispose() { }

        public bool IsFinish
        {
            get;
            protected set;
        }

        public virtual void Run()
        {

        }
    }
}