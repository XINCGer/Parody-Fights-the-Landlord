//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色的动画播放控制器(Animation版)
/// </summary>
public class CharAnimCtrl : IAnimCtrl
{
    private Animation animation;

    public CharAnimCtrl(GameObject entity)
    {
        if(null != entity)
        {
            animation = entity.GetComponent<Animation>();
        }
    }

    public void PlayAnimation(string animName)
    {
        animation.Play(animName);
    }

    public void PlayAnimation(string animName, Action<bool> callback)
    {
        //TODO：用一种比较合适的方式处理回调事件
        animation.Play(animName);
    }

    public void StopPlay()
    {
        animation.Stop();
    }

    public void Release()
    {
        if (animation.isPlaying)
        {
            animation.Stop();
        }
        animation = null;
    }

    public void PlayAnimation(int animState)
    {
    }

    public void PlayAnimation(int animState, Action<bool> callback)
    {
    }
}
