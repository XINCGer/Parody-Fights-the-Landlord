//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ColaFramework
{
    /// <summary>
    /// 场景角色的接口
    /// </summary>
    public interface ISceneCharacter : IAnimBehavior
    {
        /// <summary>
        /// 角色实际的承载的GameObject
        /// </summary>
        GameObject gameObject { get; set; }

        /// <summary>
        /// 角色的Transform缓存
        /// </summary>
        Transform transform { get; set; }

        /// <summary>
        /// 角色的位置信息
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// 角色的旋转方向
        /// </summary>
        Vector3 Rotation { get; set; }

        /// <summary>
        /// 角色的forward方向
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// 可见性
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 设置角色的位置信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        void SetPosition2D(float x, float z);

        /// <summary>
        /// 设置角色的旋转信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        void SetRotation2D(float x, float z);

        /// <summary>
        /// 角色销毁函数
        /// </summary>
        void Release();
    }
}