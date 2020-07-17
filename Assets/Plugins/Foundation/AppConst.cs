//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义一些框架内的设置
/// </summary>
public class AppConst
{
    public static bool SimulateMode = false;                       //调试/模式模式-用于在编辑器上模拟手机

    public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
    public static bool LuaBundleMode = true;                    //Lua代码AssetBundle模式
    public static bool CheckUpdate = true;                      //是否需要检测热更新

    public const string LuaBaseBundle = "lua/lua_base" + ExtName;         //包内的lua AssetBundle
    public const string LuaUpdateBundle = "lua/lua_update" + ExtName;     //热更下载的lua AseetBundle
    public const string LuaBundlePrefix = "lua/";               //lua AssetBundle的前缀
    public static List<string> LuaBundles = new List<string>() { LuaUpdateBundle, LuaBaseBundle };

    public static int GameFrameRate = 30;                        //游戏帧频

    public const string AppName = "ColaFramework";               //应用程序名称
    public const string ExtName = ".cab";                   //AssetBundle的扩展名
    public const string VersionFileName = "versions.txt";                   //版本信息文件的名称
    public static string CDNUrl = "http://10.5.102.167:6688/ColaFramework/cdn/{0}/{1}";      //CDN地址
    public static string BakCDNUrl = "http://10.5.102.167:6688/ColaFramework/cdn/{0}/{1}";      //备用CDN地址
    public const string VersionHttpUrl = "http://10.5.102.167:6688/ColaFramework/versioncontrol/{0}/{1}?p={2}&v={3}";  //版本服务器地址
    public const string BakVersionHttpUrl = "http://10.5.102.167:6688/ColaFramework/versioncontrol/{0}/{1}?p={2}&v={3}"; //备用版本服务器地址
    public const string AppDownloadUrl = "http://10.5.102.167:6688/ColaFramework/Applications";  //App的下载地址
    public const string ServerListUrl = "http://10.5.102.167:6688/ColaFramework/serverlist/{0}/{1}?p={2}&v={3}";  //下载ServerList的地址

    public static string KEY_BASE_APK_VERSION = "__PACKAGE_VERSION__";        // apk的版本号，每次换包以后这个值会被刷新，重新记录
    public static string KEY_APK_BUILD_VERSION = "__PACKAGE_BUILD_VERSION__";  // apk自带的版本号，这个配置存在Resources目录下，除非换包，否则不变
    public static string KEY_APP_CURRENT_VERSION = "__CURRENT_VERSION__";       // 当前热更的版本号
    public static string KEY_CACHE_HOTFIX_VERSION = "__CACHE_HOTFIX_VERSION__";     // 热更缓存路径的版本号
    public const int AUTO_DOWNLOAD_SIZE = 2048; // 默认2M以内热更都用数据直接下载，不提醒

    public const string SplashVideoName = "splash_video.mp4";
    public const string StoryVideoName = "story_video.mp4";

    /// <summary>
    /// 沙盒目录
    /// </summary>
    public static string AssetPath
    {
        get { return Application.persistentDataPath; }
    }

    private static string _cachePath;
    public static string CachePath
    {
        get
        {
            if (string.IsNullOrEmpty(_cachePath))
            {
                _cachePath = AssetPath + "/cache";
            }
            return _cachePath;
        }
    }

    private static string _updateCachePath;
    public static string UpdateCachePath
    {
        get
        {
            if (string.IsNullOrEmpty(_updateCachePath))
            {
                _updateCachePath = AssetPath + "/updatecache";
            }
            return _updateCachePath;
        }
    }

    private static string _dataPath;
    public static string DataPath
    {
        get
        {
            if (string.IsNullOrEmpty(_dataPath))
            {
                _dataPath = AssetPath + "/data";
            }
            return _dataPath;
        }
    }
}

