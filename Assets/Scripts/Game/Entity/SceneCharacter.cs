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
    /// 场景角色类
    /// </summary>
    public class SceneCharacter : ISceneCharacter
    {
        /// <summary>
        /// 角色动画控制器
        /// </summary>
        private IAnimCtrl animCtrl;

        private SimpleMoveController moveController;

        private AnimCtrlEnum animCtrlEnum;

        private bool isMainPlayer = false;

        public GameObject gameObject { get; set; }

        public Transform transform { get; set; }

        public string PrefabPath { get; private set; }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 Rotation
        {
            get { return transform.localRotation.eulerAngles; }
            set
            {
                Quaternion quaternion = Quaternion.Euler(value.x, value.y, value.z);
                transform.localRotation = quaternion;
            }
        }

        public Vector3 Direction
        {
            get { return transform.forward; }
            set { transform.forward = value; }
        }

        public bool Visible
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        /// <summary>
        /// 构造函数私有化，外部只能使用工厂方法接口创建
        /// </summary>
        private SceneCharacter(string path, GameObject entity, AnimCtrlEnum animCtrlEnum, bool isMainPlayer)
        {
            PrefabPath = path;
            gameObject = entity;
            transform = entity.transform;
            this.animCtrlEnum = animCtrlEnum;
            this.isMainPlayer = isMainPlayer;
            AssembleAnimCtrl();
            if (isMainPlayer)
            {
                AssembleMoveCtrl();
                PrepareMainCamera();
            }
        }

        private void AssembleAnimCtrl()
        {
            if (AnimCtrlEnum.CharAnimation == this.animCtrlEnum)
            {
                animCtrl = new CharAnimatorCtrl(gameObject);
            }
            switch (this.animCtrlEnum)
            {
                case AnimCtrlEnum.CharAnimation:
                    animCtrl = new CharAnimCtrl(gameObject);
                    break;
                case AnimCtrlEnum.CharAnimator:
                    animCtrl = new CharAnimatorCtrl(gameObject);
                    break;
                default:
                    animCtrl = new CharAnimatorCtrl(gameObject);
                    break;
            }
        }

        private void AssembleMoveCtrl()
        {
            moveController = gameObject.AddSingleComponent<SimpleMoveController>();
            moveController.Init(animCtrl);
        }

        private void PrepareMainCamera()
        {
            var camCtrl = GUIHelper.GetMainCamCtrl();
            camCtrl.target = this.transform;
        }

        /// <summary>
        /// 工厂方法，创建ISceneCharacter
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ISceneCharacter CreateSceneCharacterInf(string path, AnimCtrlEnum animCtrlEnum, bool isMainPlayer)
        {
            return CreateSceneCharacter(path, animCtrlEnum, isMainPlayer);
        }

        /// <summary>
        /// 工厂方法，创建SceneCharacter
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SceneCharacter CreateSceneCharacter(string path, AnimCtrlEnum animCtrlEnum, bool isMainPlayer)
        {
            GameObject Entity = CommonUtil.AssetTrackMgr.GetGameObject(path, null);
            return new SceneCharacter(path, Entity, animCtrlEnum, isMainPlayer);
        }

        void ISceneCharacter.SetPosition2D(float x, float z)
        {
            Position.Set(x, Position.y, z);
        }

        void ISceneCharacter.SetRotation2D(float x, float z)
        {
            transform.forward = new Vector3(x, 0, z);
        }

        void ISceneCharacter.Release()
        {
            //暂时先直接删除，后期要替换成回收到对象池
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Direction = Vector3.zero;
            transform = null;
            animCtrl.Release();

            CommonUtil.AssetTrackMgr.ReleaseGameObject(PrefabPath, gameObject);
            gameObject = null;
        }

        public void PlayAnimation(string animName)
        {
            animCtrl.PlayAnimation(animName);
        }

        public void PlayAnimation(string animName, Action<bool> callback)
        {
            animCtrl.PlayAnimation(animName, callback);
        }

        public void StopPlay()
        {
            animCtrl.StopPlay();
        }

        public void PlayAnimation(int animState)
        {
            animCtrl.PlayAnimation(animState);
        }

        public void PlayAnimation(int animState, Action<bool> callback)
        {
            animCtrl.PlayAnimation(animState, callback);
        }
    }
}