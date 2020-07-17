//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAnimBehavior
{
    void PlayAnimation(string animName);

    void PlayAnimation(string animName, Action<bool> callback);

    void PlayAnimation(int animState);

    void PlayAnimation(int animState, Action<bool> callback);

    void StopPlay();
}

/// <summary>
/// 动画控制器接口
/// </summary>
public interface IAnimCtrl : IAnimBehavior
{
    void Release();
}

public enum AnimCtrlEnum : byte
{
    CharAnimator = 1,
    CharAnimation = 2,
}

public static class AnimCurveNames
{
    public static readonly string IAnimName = "AnimEnum";

    public static readonly string Idle = "Idle";
    public static readonly string Run = "Run";
}

public static class AnimCurveEnum
{
    public static readonly int Idle = 0;
    public static readonly int Run = 1;
}
