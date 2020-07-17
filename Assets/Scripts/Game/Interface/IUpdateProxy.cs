//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以用于游戏内更新的Proxy
/// </summary>
public interface IUpdateProxy
{
    float TimeSinceUpdate { get; set; }
    void Update(float deltaTime);
}
