//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ColaFramework;
using LitJson;
using ColaFramework.Foundation;

/// <summary>
/// 通用工具类（C#端，lua端专门有Common_Util.cs供导出使用）
/// 提供一些常用功能的接口
/// </summary>
public static class CommonHelper
{

    /// <summary>
    /// 给按钮添加点击事件(以后可以往这里添加点击声音)
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void AddBtnMsg(GameObject go, Action<GameObject> callback)
    {
        if (null != go)
        {
            Button button = go.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    callback(go);
                });
            }
            else
            {
                Debug.LogWarning("该按钮没有挂载button组件！");
            }
        }
        else
        {
            Debug.LogWarning("ButtonObj为空！");
        }
    }

    /// <summary>
    /// 删除一个按钮的点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void RemoveBtnMsg(GameObject go, Action<GameObject> callback)
    {
        if (null != go)
        {
            Button button = go.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.RemoveAllListeners();
            }
            else
            {
                Debug.LogWarning("该按钮没有挂载button组件！");
            }
        }
        else
        {
            Debug.LogWarning("ButtonObj为空！");
        }
    }

    /// <summary>
    /// 根据一个预制实例化一个Prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateGoByPrefab(GameObject prefab, GameObject parent)
    {
        if (null == prefab) return null;
        GameObject obj = GameObject.Instantiate(prefab);
        if (null == obj) return null;
        obj.name = prefab.name;
        if (null != parent)
        {
            obj.transform.SetParent(parent.transform, false);
        }
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.transform.localRotation = prefab.transform.localRotation;
        obj.transform.localScale = prefab.transform.localScale;
        return obj;
    }

    /// <summary>
    /// 给物体添加一个单一组件
    /// </summary>
    /// <typeparam name="T"></typeparam>组件的类型
    /// <param name="go"></param>要添加组件的物体
    /// <returns></returns>
    public static T AddSingleComponent<T>(this GameObject go) where T : Component
    {
        if (null != go)
        {
            T component = go.GetComponent<T>();
            if (null == component)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }
        Debug.LogWarning("要添加组件的物体为空！");
        return null;
    }

    /// <summary>
    /// 获取某个物体下对应名字的子物体上的某个类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>组件的类型
    /// <param name="go"></param>父物体
    /// <param name="name"></param>子物体的名称
    /// <returns></returns>
    public static T GetComponentByName<T>(this GameObject go, string name) where T : Component
    {
        T[] components = go.GetComponentsInChildren<T>(true);
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null && components[i].name == name)
                {
                    return components[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取某个物体下子物体上某种类型的所有组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T[] GetComponentsByName<T>(this GameObject go) where T : Component
    {
        T[] components = go.GetComponentsInChildren<T>(true);

        return components;
    }

    /// <summary>
    /// 获取某个物体下对应的名字的所有子物体
    /// </summary>
    /// <param name="go"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static List<GameObject> GetGameObjectsByName(this GameObject go, string childName)
    {
        List<GameObject> list = new List<GameObject>();
        Transform[] objChildren = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < objChildren.Length; ++i)
        {
            if ((objChildren[i].name.Contains(childName)))
            {
                list.Add(objChildren[i].gameObject);
            }
        }
        return list;
    }

    /// <summary>
    /// 根据路径查找物体上的某个类型的组件(可以是自己本身/子物体/父节点)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="childPath"></param>
    /// <returns></returns>
    public static T GetComponentByPath<T>(this GameObject obj, string childPath) where T : Component
    {
        GameObject childObj = obj.FindChildByPath(childPath);
        if (null == childObj)
        {
            return null;
        }
        T component = childObj.GetComponent<T>();
        if (null == component)
        {
            Debug.LogWarning(String.Format("没有在路径:{0}上找到组件:{1}!", childPath, typeof(T)));
            return null;
        }
        return component;
    }

    /// <summary>
    /// 获取当前运行的设备平台信息
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Device And Sysinfo:\r\n");
        stringBuilder.Append(string.Format("DeviceModel: {0} \r\n", SystemInfo.deviceModel));
        stringBuilder.Append(string.Format("DeviceName: {0} \r\n", SystemInfo.deviceName));
        stringBuilder.Append(string.Format("DeviceType: {0} \r\n", SystemInfo.deviceType));
        stringBuilder.Append(string.Format("BatteryLevel: {0} \r\n", SystemInfo.batteryLevel));
        stringBuilder.Append(string.Format("DeviceUniqueIdentifier: {0} \r\n", SystemInfo.deviceUniqueIdentifier));
        stringBuilder.Append(string.Format("SystemMemorySize: {0} \r\n", SystemInfo.systemMemorySize));
        stringBuilder.Append(string.Format("OperatingSystem: {0} \r\n", SystemInfo.operatingSystem));
        stringBuilder.Append(string.Format("GraphicsDeviceID: {0} \r\n", SystemInfo.graphicsDeviceID));
        stringBuilder.Append(string.Format("GraphicsDeviceName: {0} \r\n", SystemInfo.graphicsDeviceName));
        stringBuilder.Append(string.Format("GraphicsDeviceType: {0} \r\n", SystemInfo.graphicsDeviceType));
        stringBuilder.Append(string.Format("GraphicsDeviceVendor: {0} \r\n", SystemInfo.graphicsDeviceVendor));
        stringBuilder.Append(string.Format("GraphicsDeviceVendorID: {0} \r\n", SystemInfo.graphicsDeviceVendorID));
        stringBuilder.Append(string.Format("GraphicsDeviceVersion: {0} \r\n", SystemInfo.graphicsDeviceVersion));
        stringBuilder.Append(string.Format("GraphicsMemorySize: {0} \r\n", SystemInfo.graphicsMemorySize));
        stringBuilder.Append(string.Format("GraphicsMultiThreaded: {0} \r\n", SystemInfo.graphicsMultiThreaded));
        stringBuilder.Append(string.Format("SupportedRenderTargetCount: {0} \r\n", SystemInfo.supportedRenderTargetCount));
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获取设备的电量
    /// </summary>
    /// <returns></returns>
    public static float GetBatteryLevel()
    {
        if (Application.isMobilePlatform)
        {
            return SystemInfo.batteryLevel;
        }

        return 1;
    }

    /// <summary>
    /// 获取设备的电池状态
    /// </summary>
    /// <returns></returns>
    public static BatteryStatus GetBatteryStatus()
    {
        return SystemInfo.batteryStatus;
    }

    /// <summary>
    /// 获取设备网络的状况
    /// </summary>
    /// <returns></returns>
    public static NetworkReachability GetNetworkStatus()
    {
        return Application.internetReachability;
    }

    /// <summary>
    /// 检测一个功能模块是否开启
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="isTips"></param>
    /// <returns></returns>
    public static CheckFuncResult CheckFuncOpen(string funcName, bool isTips)
    {
        //todo:做些检查工作
        CheckFuncResult result = CheckFuncResult.True;
        if (CheckFuncResult.True != result && isTips)
        {
            switch (result)
            {
                case CheckFuncResult.False:
                    Debug.LogWarning(string.Format("功能未开启{0}", funcName));
                    break;
                case CheckFuncResult.LevelLimit:
                    Debug.LogWarning(string.Format("等级限制不能开启{0}", funcName));
                    break;
                case CheckFuncResult.TimeLimit:
                    Debug.LogWarning(string.Format("时间限制不能开启{0}", funcName));
                    break;
                case CheckFuncResult.None:
                    Debug.LogWarning(string.Format("未知原因不能开启{0}", funcName));
                    break;
                default:
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// 将世界坐标转化UGUI坐标
    /// </summary>
    /// <param name="gameCamera"></param>
    /// <param name="canvas"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector2 WorldToUIPoint(Camera gameCamera, Canvas canvas, Vector3 worldPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            gameCamera.WorldToScreenPoint(worldPos), canvas.worldCamera, out pos);
        return pos;
    }

    /// <summary>
    /// 判断一个string数组中是否包含某个string
    /// </summary>
    /// <param name="key"></param>
    /// <param name="strList"></param>
    /// <returns></returns>
    public static bool IsArrayContainString(string key, params string[] strList)
    {
        if (null == key || null == strList)
        {
            return false;
        }
        for (int i = 0; i < strList.Length; i++)
        {
            if (strList[i].Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取主机IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetHostIp()
    {
        String url = "http://hijoyusers.joymeng.com:8100/test/getNameByOtherIp";
        string IP = "未获取到外网ip";
        try
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = Encoding.Default;
            string str = client.DownloadString(url);
            client.Dispose();

            if (!str.Equals("")) IP = str;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        Debug.Log("get host ip :" + IP);
        return IP;
    }

    /// <summary>
    /// 获取当前位置距离地面的高度
    /// </summary>
    /// <param name="vPos"></param>
    /// <param name="fRadius"></param>
    /// <returns></returns>
    public static float GetTerrainHeight(Vector3 vPos, float fRadius = 0)
    {
        var orign = vPos;
        orign.y += 300;
        float maxDistance = vPos.y + 400;
        RaycastHit hitInfo;
        bool bRet = (fRadius == 0) ? Physics.Raycast(orign, Vector3.down, out hitInfo, maxDistance, LayerMask.GetMask("Terrain")) : Physics.SphereCast(orign, fRadius, Vector3.down, out hitInfo, orign.y + 50, LayerMask.GetMask("Terrain"));
        return bRet ? hitInfo.point.y : 0;
    }

    /// <summary>
    /// 获取导航点距离地面的高度
    /// </summary>
    /// <param name="vPos"></param>
    /// <returns></returns>
    public static float GetNavMeshHeight(Vector3 vPos)
    {
        return GetTerrainHeight(vPos);
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="coroutine"></param>
    public static void StartCoroutine(IEnumerator coroutine)
    {
        GameLauncher.Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="methodName"></param>
    public static void StartCoroutine(string methodName, [UnityEngine.Internal.DefaultValue("null")] object value)
    {
        GameLauncher.Instance.StartCoroutine(methodName, value);
    }

    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="methodName"></param>
    public static void StopCoroutine(string methodName)
    {
        GameLauncher.Instance.StopCoroutine(methodName);
    }

    /// <summary>
    /// 停止所有的协程
    /// </summary>
    public static void StopAllCoroutines()
    {
        GameLauncher.Instance.StopAllCoroutines();
    }

    /// <summary>
    /// UI文字组件的打字机效果(不支持换行)
    /// </summary>
    /// <param name="textCommponent"></param>
    /// <param name="content"></param>
    /// <param name="delta"></param>
    /// <param name="onComplete"></param>
    public static void TextTyperEffect(Text textCommponent, string content, float delta, Action onComplete)
    {
        if (null != textCommponent && !string.IsNullOrEmpty(content))
        {
            textCommponent.text = "";
            int length = 1;
            long timerID = -1;

            Action killTimer = () =>
            {
                if (-1 != timerID)
                {
                    Timer.Cancel(timerID);
                }
            };

            timerID = Timer.RunBySeconds(delta, () =>
             {
                 var subContent = content.Substring(0, length);
                 textCommponent.text = subContent;
                 length++;
                 if (length > content.Length)
                 {
                     killTimer();
                     if (null != onComplete)
                     {
                         onComplete();
                     }
                 }
             }, null);
        }
    }

    /// <summary>
    /// 检查本地文件是否存在,如果目录不存在则创建目录
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckLocalFileExist(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }
        string dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            return false;
        }

        if (File.Exists(filePath))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 资源清理和垃圾回收
    /// </summary>
    public static void ClearMemory()
    {
        GameManager.Instance.GetLuaClient().LuaGC();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    /// <summary>
    /// 执行Lua方法
    /// </summary>
    /// <returns></returns>
    public static object[] CallLuaMethod(string module, string func, params object[] args)
    {
        LuaClient luaClient = GameManager.Instance.GetLuaClient();
        if (null == luaClient) return null;
        return luaClient.CallFunction(module + "." + func, args);
    }

    /// <summary>
    /// 切割一张Texure大图上面的一部分，作为Sprite精灵返回
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Sprite SliceTextureToSprite(Texture2D texture2D, float x, float y, float width, float height)
    {
        if (null != texture2D)
        {
            if (x + width > texture2D.width)
            {
                width = texture2D.width - x;
                Debug.LogWarning("the width is larger then texture2D width!");
            }
            if (y + height > texture2D.height)
            {
                height = texture2D.height - y;
                Debug.LogWarning("the height is larger then texture2D height!");
            }

            Sprite sprite = Sprite.Create(texture2D, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        Debug.LogWarning("Texture2D 不能为空！");
        return null;
    }

    /// <summary>
    /// App基准版本号（表示当前或者历史安装的apk的version）
    /// </summary>
    public static string PackageVersion
    {
        get
        {
            var baseVersion = PlayerPrefs.GetString(AppConst.KEY_BASE_APK_VERSION, null);
            if (string.IsNullOrEmpty(baseVersion))
            {
                var textAsset = Resources.Load<TextAsset>("app_version");
                if (null != textAsset)
                {
                    var appVersion = JsonMapper.ToObject<AppVersion>(textAsset.text);
                    PlayerPrefs.SetString(AppConst.KEY_BASE_APK_VERSION, appVersion.Version);
                    baseVersion = appVersion.Version;
                }
                Resources.UnloadAsset(textAsset);
            }
            return baseVersion;
        }
        set
        {
            PlayerPrefs.SetString(AppConst.KEY_BASE_APK_VERSION, value);
        }
    }

    /// <summary>
    /// App基准版本号（表示当前或者历史安装的apk的version）
    /// </summary>
    public static int[] PackageVersionInt
    {
        get
        {
            var version = PackageVersion;
            var versionStrs = version.Split('.');
            var versionInts = new int[versionStrs.Length];
            for (int i = 0; i < versionStrs.Length; i++)
            {
                versionInts[i] = Convert.ToInt32(versionStrs[i]);
            }
            return versionInts;
        }
    }

    /// <summary>
    /// 当前APP的版本号
    /// </summary>
    public static string HotUpdateVersion
    {
        get
        {
            var baseVersion = PlayerPrefs.GetString(AppConst.KEY_APP_CURRENT_VERSION, null);
            if (string.IsNullOrEmpty(baseVersion))
            {
                baseVersion = PackageVersion;
            }
            return baseVersion;
        }
        set
        {
            PlayerPrefs.SetString(AppConst.KEY_APP_CURRENT_VERSION, value);
        }
    }

    /// <summary>
    /// 当前APP的版本号
    /// </summary>
    public static int[] HotUpdateVersionInt
    {
        get
        {
            var version = HotUpdateVersion;
            var versionStrs = version.Split('.');
            var versionInts = new int[versionStrs.Length];
            for (int i = 0; i < versionStrs.Length; i++)
            {
                versionInts[i] = Convert.ToInt32(versionStrs[i]);
            }
            return versionInts;
        }
    }

    /// <summary>
    /// apk自带的包内Build版本号
    /// </summary>
    public static string APKBuildVersion
    {
        get
        {
            string baseVersion = "0.0.0.0";
            var textAsset = Resources.Load<TextAsset>("app_version");
            if (null != textAsset)
            {
                var appVersion = JsonMapper.ToObject<AppVersion>(textAsset.text);
                baseVersion = appVersion.Version;
            }
            Resources.UnloadAsset(textAsset);
            return baseVersion;
        }
    }

    /// <summary>
    /// 对比版本号
    /// 大于返回 1
    /// 小于返回 -1
    /// 等于返回 0
    /// </summary>
    /// <param name="baseV"></param>
    /// <param name="newV"></param>
    /// <returns></returns>
    public static int CompareVersion(string baseV, string newV)
    {
        var baseVs = baseV.Split('.');
        var newVs = newV.Split('.');

        for (int i = 0; i < baseVs.Length; i++)
        {
            var a = Convert.ToInt32(baseVs[i]);
            var b = Convert.ToInt32(newVs[i]);
            if (b > a)
            {
                return -1;
            }
            else if (a > b)
            {
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 判断值是否和缓存中的一致
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValueEqualPrefs(string key, string value)
    {
        string prefVal = PlayerPrefs.GetString(key, "");
        Debug.LogFormat("IsValueEqualPrefs, key:{0}, val:{1}, compare val:{2}", key, prefVal, value);
        return prefVal == value;
    }

    /// <summary>
    /// 格式化bytes为标准字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string FormatByte(long bytes)
    {
        if (bytes < 1024)
        {
            return bytes.ToString() + "B";
        }
        long num = bytes / 1024;
        if (num < 1024)
        {
            return string.Format("{0:F2}KB", bytes * 1.0f / 1024);
        }
        return string.Format("{0:F2}MB", bytes * 1.0f / 1048576);
    }

    /// <summary>
    /// 格式化KB为标准字符串
    /// </summary>
    /// <param name="kb"></param>
    /// <returns></returns>
    public static string FormatKB(float kb)
    {
        if (kb < 1024)
        {
            return string.Format("{0:F2}KB", kb);
        }
        return string.Format("{0:F2}MB", kb / 1024);
    }

    /// <summary>
    /// GetSystemArchType函数的反值，用来排除热更文件用
    /// </summary>
    /// <returns></returns>
    public static int GetNotSystemArchType()
    {
        if (Application.isMobilePlatform)
        {
            return System.IntPtr.Size == 8 ? 1 : 2;
        }

        return -1;
    }
}

