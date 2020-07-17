//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColaFramework.Foundation;

namespace ColaFramework
{

    /// <summary>
    /// 角色的动画播放控制器(Animator版)
    /// </summary>
    public class CharAnimatorCtrl : IAnimCtrl
    {
        private Animator animator;

        public CharAnimatorCtrl(GameObject entity)
        {
            animator = entity.AddSingleComponent<Animator>();
            animator.runtimeAnimatorController = CommonUtil.AssetTrackMgr.GetAsset<RuntimeAnimatorController>(Constants.ModelAnimatorPath + entity.name + ".controller");
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        public void PlayAnimation(string animName)
        {
        }

        public void PlayAnimation(string animName, Action<bool> callback)
        {
        }

        public void PlayAnimation(int animState)
        {
            animator.SetInteger(AnimCurveNames.IAnimName, animState);
        }

        public void PlayAnimation(int animState, Action<bool> callback)
        {
            //用一种合适的方式触发回调
            animator.SetInteger(AnimCurveNames.IAnimName, animState);
        }

        public void Release()
        {
            StopPlay();
            animator = null;
        }

        public void StopPlay()
        {
            animator.SetBool("Idle", true);
        }
    }
}
