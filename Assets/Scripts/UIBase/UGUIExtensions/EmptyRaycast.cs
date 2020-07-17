//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    /// <summary>
    /// 用于使空白的UI可以进行射线检测的组件
    /// </summary>
    public class EmptyRaycast : MaskableGraphic
    {
        public void EnableRayCast(bool enable)
        {
            this.raycastTarget = enable;
        }
        protected EmptyRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}