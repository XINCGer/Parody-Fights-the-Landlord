//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColaFramework.NetWork;
using ColaFramework.Foundation;

namespace ColaFramework
{
    /// <summary>
    /// 游戏核心管理中心
    /// </summary>
    public class GameManager
    {

        private static GameManager instance;
        private bool init = false;

        /// <summary>
        /// Launcher的Obj
        /// </summary>
        private GameObject gameLauncherObj;

        /// <summary>
        /// 场景/关卡管理器
        /// </summary>
        private SceneMgr sceneMgr;

        /// <summary>
        /// 音频管理器
        /// </summary>
        private AudioManager audioManager;

        /// <summary>
        /// 计时器管理器
        /// </summary>
        private TimerManager timerManager;

        /// <summary>
        /// 网络消息处理器
        /// </summary>
        private NetMessageCenter netMessageCenter;

        /// <summary>
        /// 定时GC与垃圾回收策略管理器
        /// </summary>
        private AutoResGCMgr autoResGCMgr;

        private InputMgr inputMgr;

        private LuaClient luaClient;

        private GameManager()
        {

        }

        public static GameManager Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// 初始化游戏核心
        /// </summary>
        public void InitGameCore(GameObject gameObject)
        {
            init = false;

            AssetLoader.Initialize(() =>
            {
                gameLauncherObj = gameObject;
                sceneMgr = gameObject.AddComponent<SceneMgr>();
                audioManager = AudioManager.Instance;
                timerManager = TimerManager.Instance;
                inputMgr = gameLauncherObj.AddComponent<InputMgr>();
                netMessageCenter = NetMessageCenter.Instance;
                autoResGCMgr = AutoResGCMgr.Instance;

                CommonUtil.Initialize();
                GameStart();
            }, (error) => { Debug.Log(error); });

        }

        /// <summary>
        /// 游戏模块开始运行入口
        /// </summary>
        public void GameStart()
        {
            audioManager.Init();
            timerManager.Init();
            netMessageCenter.Init();

            //将lua初始化移动到这里，所有的必要条件都准备好以后再初始化lua虚拟机
            luaClient = gameLauncherObj.AddComponent<LuaClient>();

            init = true;
        }

        /// <summary>
        /// 模拟 Update
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!init) return;
            timerManager.Update(deltaTime);
            AssetLoader.Update(deltaTime);
            audioManager.Update(deltaTime);
            netMessageCenter.Update(deltaTime);
            autoResGCMgr.Update(deltaTime);
        }

        /// <summary>
        /// 模拟 LateUpdate
        /// </summary>
        /// <param name="deltaTime"></param>
        public void LateUpdate(float deltaTime)
        {
            if (!init) return;
        }

        /// <summary>
        /// 模拟 FixedUpdate
        /// </summary>
        /// <param name="fixedDeltaTime"></param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!init) return;
        }

        public void OnApplicationQuit()
        {

        }

        public void OnApplicationPause(bool pause)
        {

        }

        public void OnApplicationFocus(bool focus)
        {

        }

        public SceneMgr GetSceneMgr()
        {
            if (null != sceneMgr)
            {
                return sceneMgr;
            }
            Debug.LogWarning("sceneMgr构造异常");
            return null;
        }

        public LuaClient GetLuaClient()
        {
            if (null != luaClient)
            {
                return luaClient;
            }
            Debug.LogWarning("luaClient构造异常");
            return null;
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void ApplicationQuit()
        {
            Application.Quit();
        }

    }
}

