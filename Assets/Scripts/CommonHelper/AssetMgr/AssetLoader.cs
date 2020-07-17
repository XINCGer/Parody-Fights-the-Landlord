//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Plugins.XAsset;
using Object = UnityEngine.Object;

namespace ColaFramework.Foundation
{
    /// <summary>
    /// 资源加载的对外接口，封装平台和细节，可对Lua导出
    /// </summary>
    public static class AssetLoader
    {
        private const int CHECK_INTERVAL = 10;
        private static float time = 0f;
        private static Dictionary<string, WeakReference> AssetReferences = new Dictionary<string, WeakReference>(32);
        private static Dictionary<string, Asset> LoadedAssets = new Dictionary<string, Asset>(32);
        private static List<string> UnUsedAssets = new List<string>(16);

        /// <summary>
        /// 根据类型和路径返回相应的资源(同步方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : Object
        {
            return Load(path, typeof(T)) as T;
        }

        /// <summary>
        /// 根据类型和路径返回相应的资源(同步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object Load(string path, Type type)
        {
#if UNITY_EDITOR 
            //是否开启了Editor下模拟模式
            if (AppConst.SimulateMode)
            {
                return LoadInternal(path, type);
            }
            path = Constants.GameAssetBasePath + path;
            if (Path.HasExtension(path))
            {
                return AssetDatabase.LoadAssetAtPath(path, type);
            }
            else
            {
                Debug.LogWarning("资源加载的路径不合法!");
                return null;
            }
#else
            return LoadInternal(path, type);
#endif
        }

        private static Object LoadInternal(string path, Type type)
        {
            WeakReference wkRef = null;
            if (AssetReferences.TryGetValue(path, out wkRef))
            {
                if (CheckAssetAlive(wkRef.Target))
                {
                    return wkRef.Target as Object;
                }
            }
            var assetProxy = Assets.Load(path, type);
            var asset = assetProxy.asset;
            assetProxy.ClearAsset();
            wkRef = new WeakReference(asset);
            Asset assetRef = null;
            if (LoadedAssets.TryGetValue(path, out assetRef))
            {
                LoadedAssets[path] = assetProxy;
                Assets.Unload(assetRef);
            }
            else
            {
                LoadedAssets.Add(path, assetProxy);
            }
            AssetReferences[path] = wkRef;
            return asset;
        }


        /// <summary>
        /// 根据类型和路径返回相应的资源(异步方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public static void LoadAsync<T>(string path, Action<Object> callback) where T : Object
        {
            LoadAsync(path, typeof(T), callback);
        }

        /// <summary>
        /// 根据类型和路径返回相应的资源(异步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="t"></param>
        public static void LoadAsync(string path, Type type, Action<Object> callback)
        {
#if UNITY_EDITOR
            //是否开启了Editor下模拟模式
            if (AppConst.SimulateMode)
            {
                LoadAsyncInternal(path, type, callback);
                return;
            }
            //模拟异步
            var asset = Load(path, type);
            callback(asset);
#else
            LoadAsyncInternal(path, type,callback);
#endif
        }

        public static void LoadAsyncInternal(string path, Type type, Action<Object> callback)
        {
            WeakReference wkRef = null;
            if (AssetReferences.TryGetValue(path, out wkRef))
            {
                if (CheckAssetAlive(wkRef.Target))
                {
                    callback(wkRef.Target as Object);
                }
            }
            var assetProxy = Assets.LoadAsync(path, type);
            assetProxy.completed += (obj) =>
            {
                wkRef.Target = obj.asset;
                var asset = obj.asset;
                Asset assetRef = null;
                if (LoadedAssets.TryGetValue(path, out assetRef))
                {
                    LoadedAssets[path] = assetProxy;
                    Assets.Unload(assetRef);
                }
                else
                {
                    LoadedAssets.Add(path, assetProxy);
                }
                obj.ClearAsset();
                callback(asset);
            };
        }

#if UNITY_EDITOR
        [LuaInterface.NoToLua]
        public static void LoadAllAssetsAtPath(string path, out Object[] objects)
        {
            objects = AssetDatabase.LoadAllAssetsAtPath(path);
        }
#endif

        private static bool CheckAssetAlive(System.Object asset)
        {
            if (null == asset) { return false; }
            if (asset is Object)
            {
                Object UnityObject = asset as Object;
                if (null == UnityObject || !UnityObject)
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(string.Format("InVaild Asset Type:{0}", asset.GetType()));
            }
            return true;
        }

        public static void Update(float deltaTime)
        {
            time += deltaTime;
            if (time < CHECK_INTERVAL) { return; }
            time = 0;
            foreach (KeyValuePair<string, WeakReference> kvPair in AssetReferences)
            {
                if (false == CheckAssetAlive(kvPair.Value.Target))
                {
                    UnUsedAssets.Add(kvPair.Key);
                }
            }
            if (UnUsedAssets.Count > 0)
            {
                foreach (var name in UnUsedAssets)
                {
                    Debug.Log("卸载无用资源:" + name);
                    AssetReferences.Remove(name);
                    Asset asset = null;
                    if (LoadedAssets.TryGetValue(name, out asset))
                    {
                        Assets.Unload(asset);
                        LoadedAssets.Remove(name);
                    }
                }
                UnUsedAssets.Clear();
            }
        }

        public static void Initialize(Action onSuccess, Action<string> onError)
        {
            time = 0;
            Assets.Initialize(onSuccess, onError);
        }

        public static void Release()
        {
            time = 0;
            //强制卸载所有的资源

        }
    }

}