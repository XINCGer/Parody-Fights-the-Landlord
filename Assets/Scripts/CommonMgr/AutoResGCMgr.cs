//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework
{
    public class AutoResGCMgr : Singleton<AutoResGCMgr>
    {
        public int AutoGCInterval = 30;
        private float gcTick = 0;

        public void Update(float dt)
        {
            gcTick += dt;
            if(gcTick >= AutoGCInterval)
            {
                CommonHelper.ClearMemory();
                gcTick = 0;
            }
        }
    }
}
