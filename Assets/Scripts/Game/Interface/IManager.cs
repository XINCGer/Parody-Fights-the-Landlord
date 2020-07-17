//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// 管理器的接口定义
/// </summary>
public interface IManager : IUpdateProxy, IDisposable
{
    /// <summary>
    /// 管理器初始化
    /// </summary>
    void Init();
}
