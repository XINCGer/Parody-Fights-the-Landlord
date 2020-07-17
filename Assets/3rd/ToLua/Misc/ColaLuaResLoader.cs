//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using System.IO;
using Plugins.XAsset;

/// <summary>
/// 自定义的Lua加载器
/// </summary>
public class ColaLuaResLoader : LuaFileUtils
{

    public ColaLuaResLoader()
    {
        instance = this;
        beZip = AppConst.LuaBundleMode;
    }

    public override byte[] ReadFile(string fileName)
    {
        return base.ReadFile(fileName);
    }

    /// <summary>
    /// 添加打入Lua代码的AssetBundle
    /// </summary>
    /// <param name="bundle"></param>
    public void AddBundle(string bundleName)
    {
        var url = bundleName.ToLower();
        var bundle = Assets.LoadBundle(url);
        if(null != bundle)
        {
            base.AddSearchBundle(url, bundle);
        }
        else
        {
            Debug.LogError(string.Format("{0} : Luabundle is load failed!",bundleName));
        }
    }
}
