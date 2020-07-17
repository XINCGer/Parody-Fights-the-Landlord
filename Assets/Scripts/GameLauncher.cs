//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using ColaFramework;
using ColaFramework.Foundation;

/// <summary>
/// 游戏入口:游戏启动器类
/// </summary>
public class GameLauncher : MonoBehaviour
{
    private static GameLauncher instance;
    private GameManager gameManager;
    private GameObject fpsHelperObj;
    private FPSHelper fpsHelper;
    private LogHelper logHelper;
    private InputMgr inputMgr;

    public static GameLauncher Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// LogHelper实例
    /// </summary>
    public LogHelper LogHelper
    {
        get { return this.logHelper; }
    }

    void Awake()
    {
        instance = this;
        gameManager = GameManager.Instance;
        DOTween.Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_STANDALONE_WIN
            Screen.SetResolution(1280, 720, false);
#endif
        Application.targetFrameRate = AppConst.GameFrameRate;

        DontDestroyOnLoad(gameObject);

        //加入输入输出管理器
        inputMgr = gameObject.AddComponent<InputMgr>();

#if SHOW_FPS
        fpsHelperObj = new GameObject("FpsHelperObj");
        fpsHelper = fpsHelperObj.AddComponent<FPSHelper>();
        GameObject.DontDestroyOnLoad(fpsHelperObj);
#endif

#if BUILD_DEBUG_LOG || UNITY_EDITOR
#if UNITY_2017_1_OR_NEWER
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = true;
#endif
#else
#if UNITY_2017_1_OR_NEWER
        Debug.unityLogger.logEnabled = false;
#else
        Debug.logger.logEnabled = false;
#endif
#endif

#if OUTPUT_LOG
        GameObject logHelperObj = new GameObject("LogHelperObj");
        logHelper = logHelperObj.AddComponent<LogHelper>();
        GameObject.DontDestroyOnLoad(logHelperObj);

        Application.logMessageReceived += logHelper.LogCallback;
#endif
        //初始化多线程工具
        ColaLoom.Initialize();
    }

    // Use this for initialization
    void Start()
    {
        TaskManager.Instance.Init(this);
        DownloadPatcher.Instance.StartUpdate(OnDownloadPathDone);
    }

    /// <summary>
    /// needUnpack 有热更到资源的话就要解压 没有就不需要
    /// </summary>
    /// <param name="needUnpack"></param>
    private void OnDownloadPathDone(bool needUnpack)
    {
        Debug.Log("热更完成：" + needUnpack);
        StartCoroutine(InitGameCore());
    }

    void Update()
    {
        if (null != ColaHelper.Update)
        {
            ColaHelper.Update(Time.deltaTime);
        }
        gameManager.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (null != ColaHelper.LateUpdate)
        {
            ColaHelper.LateUpdate(Time.deltaTime);
        }
        gameManager.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (null != ColaHelper.FixedUpdate)
        {
            ColaHelper.FixedUpdate(Time.fixedDeltaTime);
        }
        gameManager.FixedUpdate(Time.fixedDeltaTime);
    }


    private void OnApplicationQuit()
    {
        if (null != ColaHelper.OnApplicationQuit)
        {
            ColaHelper.OnApplicationQuit();
        }
        gameManager.OnApplicationQuit();
    }

    private void OnApplicationPause(bool pause)
    {
        if (null != ColaHelper.OnApplicationPause)
        {
            ColaHelper.OnApplicationPause(pause);
        }
        gameManager.OnApplicationPause(pause);
    }

    private void OnApplicationFocus(bool focus)
    {
        gameManager.OnApplicationFocus(focus);
    }

    public void ApplicationQuit(string exitCode = "0")
    {
        gameManager.ApplicationQuit();
    }

    IEnumerator InitGameCore()
    {
        yield return null;
        gameManager.InitGameCore(gameObject);
    }

    public void DelayInvokeNextFrame(System.Action action)
    {
        StartCoroutine(InvokeNextFrame(action));
    }

    private IEnumerator InvokeNextFrame(System.Action action)
    {
        yield return 1;
        if (null != action)
        {
            action();
        }
    }

    public void DelayInvokeSeconds(float seconds, System.Action action)
    {
        StartCoroutine(InvokeSeconds(seconds, action));
    }

    private IEnumerator InvokeSeconds(float seconds, System.Action action)
    {
        yield return new WaitForSeconds(seconds);
        if (null != action)
        {
            action();
        }
    }
}


