/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;
using System.IO;
using System;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class LuaClient : MonoBehaviour
{
    public static LuaClient Instance
    {
        get;
        protected set;
    }

    protected LuaState luaState = null;
    protected ColaLuaResLoader luaLoader;
    protected LuaLooper loop = null;
    protected LuaFunction levelLoaded = null;

    protected bool openLuaSocket = false;
    protected bool beZbStart = false;

    protected void Awake()
    {
        Instance = this;
        Init();

#if UNITY_5_4_OR_NEWER
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif        
    }

    protected void Init()
    {
        luaLoader = new ColaLuaResLoader();
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        Bind();
        InitLuaPath();
        InitLuaBundle();
        LoadLuaFiles();
    }

    protected virtual void InitLuaPath()
    {
#if UNITY_EDITOR
        if (!Directory.Exists(LuaConst.luaDir))
        {
            string msg = string.Format("luaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.luaDir);
            throw new LuaException(msg);
        }

        if (!Directory.Exists(LuaConst.toluaDir))
        {
            string msg = string.Format("toluaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.toluaDir);
            throw new LuaException(msg);
        }

        luaState.AddSearchPath(LuaConst.toluaDir);
        luaState.AddSearchPath(LuaConst.luaDir);
#endif
        if (!AppConst.LuaBundleMode)
        {
            luaState.AddSearchPath(LuaConst.luaResDir);
            luaState.AddSearchPath(LuaConst.streamingAssetLua);
        }
    }

    protected virtual void InitLuaBundle()
    {
        if (AppConst.LuaBundleMode)
        {
            foreach(var bundleName in AppConst.LuaBundles)
            {
                luaLoader.AddBundle(bundleName);
            }
        }
    }

    protected virtual void LoadLuaFiles()
    {
        OnLoadFinished();
    }

    /// <summary>
    /// 初始化加载第三方库
    /// </summary>
    protected virtual void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_sproto_core);
        luaState.OpenLibs(LuaDLL.luaopen_protobuf_c);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
        luaState.OpenLibs(LuaDLL.luaopen_bit);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif

        if (LuaConst.openLuaSocket)
        {
            luaState.OpenLibs(LuaDLL.luaopen_socket_core);
            OpenLuaSocket();
        }

        if (LuaConst.openLuaDebugger)
        {
            OpenZbsDebugger();
        }
        OpenCJson();
    }

    public void OpenZbsDebugger(string ip = "localhost")
    {
        if (!Directory.Exists(LuaConst.zbsDir))
        {
            Debugger.LogWarning("ZeroBraneStudio not install or LuaConst.zbsDir not right");
            return;
        }

        if (!LuaConst.openLuaSocket)
        {
            OpenLuaSocket();
        }

        if (!string.IsNullOrEmpty(LuaConst.zbsDir))
        {
            luaState.AddSearchPath(LuaConst.zbsDir);
        }

        luaState.LuaDoString(string.Format("DebugServerIp = '{0}'", ip), "@LuaClient.cs");
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Socket_Core(IntPtr L)
    {
        return LuaDLL.luaopen_socket_core(L);
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Mime_Core(IntPtr L)
    {
        return LuaDLL.luaopen_mime_core(L);
    }

    protected void OpenLuaSocket()
    {
        LuaConst.openLuaSocket = true;

        luaState.BeginPreLoad();
        luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
        luaState.RegFunction("mime.core", LuaOpen_Mime_Core);
        luaState.EndPreLoad();
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        luaState.OpenLibs(LuaDLL.luaopen_cjson);
        luaState.LuaSetField(-2, "cjson");

        luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
        luaState.LuaSetField(-2, "cjson.safe");
    }

    protected virtual void CallMain()
    {
        LuaFunction main = luaState.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;
    }

    protected virtual void StartMain()
    {
        luaState.DoFile("Main.lua");
        levelLoaded = luaState.GetFunction("OnLevelWasLoaded");
        CallMain();
    }

    protected void StartLooper()
    {
        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = luaState;
    }

    protected virtual void Bind()
    {
        LuaBinder.Bind(luaState);
        ColaLuaExtension.Register(luaState);
        DelegateFactory.Init();
        LuaCoroutine.Register(luaState, this);
    }

    protected virtual void OnLoadFinished()
    {
        luaState.Start();
        StartLooper();
        StartMain();
    }

    /// <summary>
    /// 执行一个lua File
    /// </summary>
    /// <param name="filename"></param>
    public void DoFile(string filename)
    {
        luaState.DoFile(filename);
    }

    // Update is called once per frame
    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = luaState.GetFunction(funcName);
        if (func != null)
        {
            return func.LazyCall(args);
        }
        return null;
    }

    public void LuaGC()
    {
        luaState.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    void OnLevelLoaded(int level)
    {
        if (levelLoaded != null)
        {
            levelLoaded.BeginPCall();
            levelLoaded.Push(level);
            levelLoaded.PCall();
            levelLoaded.EndPCall();
        }

        if (luaState != null)
        {
            luaState.RefreshDelegateMap();
        }
    }

#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLevelLoaded(scene.buildIndex);
    }
#else
    protected void OnLevelWasLoaded(int level)
    {
        OnLevelLoaded(level);
    }
#endif

    public virtual void Destroy()
    {
        if (luaState != null)
        {
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif    
            luaState.Call("OnApplicationQuit", false);
            DetachProfiler();
            LuaState state = luaState;
            luaState = null;

            if (levelLoaded != null)
            {
                levelLoaded.Dispose();
                levelLoaded = null;
            }

            if (loop != null)
            {
                loop.Destroy();
                loop = null;
            }

            state.Dispose();
            Instance = null;
        }
    }

    protected void OnDestroy()
    {
        Destroy();
    }

    protected void OnApplicationQuit()
    {
        Destroy();
    }

    public static LuaState GetMainState()
    {
        return Instance.luaState;
    }

    public LuaLooper GetLooper()
    {
        return loop;
    }

    LuaTable profiler = null;

    public void AttachProfiler()
    {
        if (profiler == null)
        {
            profiler = luaState.Require<LuaTable>("UnityEngine.Profiler");
            profiler.Call("start", profiler);
        }
    }
    public void DetachProfiler()
    {
        if (profiler != null)
        {
            profiler.Call("stop", profiler);
            profiler.Dispose();
            LuaProfiler.Clear();
        }
    }
}
