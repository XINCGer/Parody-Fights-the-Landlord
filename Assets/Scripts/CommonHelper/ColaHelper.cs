//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework
{
    /// <summary>
    /// ColaHelper
    /// </summary>
    public static class ColaHelper
    {
        public static Action<float> Update;
        public static Action<float> LateUpdate;
        public static Action<float> FixedUpdate;
        public static Action OnApplicationQuit;
        public static Action<bool> OnApplicationPause;
    }
}
