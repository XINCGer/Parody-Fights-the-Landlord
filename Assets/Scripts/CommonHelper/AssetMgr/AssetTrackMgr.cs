//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColaFramework.Foundation
{
    public delegate Object InstantiateAction(Object obj);

    public interface IAssetTrackMgr
    {
        void SetCapcitySize(string group, int capcity);

        void SetDisposeInterval(string group, int disposeTimeInterval);

        void Release();

        GameObject GetGameObject(string path, Transform parent);

        void ReleaseGameObject(string path, GameObject gameObject);

        void DiscardGameObject(string path, GameObject gameObject);

        T GetAsset<T>(string path) where T : Object;

        Object GetAsset(string path, Type type);

        void ReleaseAsset(string path, Object obj);
    }

    public class AssetTrackMgr : IAssetTrackMgr
    {
        #region CONST
        public const int DISPOSE_TIME_VALUE = 15;
        public const int DISPOSE_CHECK_INTERVAL = 5;
        public const int CAPCITY_SIZE = 15;

        private const int ILLEGAL_VALUE = -1;
        #endregion

        #region Params
        public Transform rootTF { get; private set; }
        public InstantiateAction instantiateAction { get; private set; }

        private ContainerPool containerPool;

        private Dictionary<string, AssetContainer> assetContainerMap = new Dictionary<string, AssetContainer>();
        private Dictionary<string, GameObjectContainer> gameObjectContainerMap = new Dictionary<string, GameObjectContainer>();

        private Dictionary<string, int> capcityValueMap = new Dictionary<string, int>();
        private Dictionary<string, int> disposeTimeMap = new Dictionary<string, int>();

        private int G_Capcity = ILLEGAL_VALUE;
        private int G_DisposeTime = ILLEGAL_VALUE;
        #endregion

        #region public interface implements
        public AssetTrackMgr(Transform rootTransform = null, InstantiateAction action = null)
        {
            rootTF = null == rootTransform ? new GameObject("AssetTrackRoot").transform : rootTransform;
            instantiateAction = null == action ? GameObject.Instantiate : action;
            containerPool = new ContainerPool();
            GameObject.DontDestroyOnLoad(rootTF.gameObject);
            rootTF.gameObject.AddSingleComponent<ContainerMapChecker>().assetTrackMgr = this;
        }

        public void DiscardGameObject(string path, GameObject gameObject)
        {
            GameObjectContainer container = null;
            if (gameObjectContainerMap.TryGetValue(path, out container))
            {
                container.Discard(gameObject);
            }
            else
            {
                Debug.LogError("AssetTrackMgr gameobject is not created by container pool,just destory it! " + path);
                GameObject.Destroy(gameObject);
            }
        }

        public T GetAsset<T>(string path) where T : Object
        {
            return GetAsset(path, typeof(T)) as T;
        }

        public Object GetAsset(string path, Type type)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetTrackMgr GetAsset param path is null or empty!");
                return null;
            }

            Object obj = null;
            AssetContainer container;
            if (assetContainerMap.TryGetValue(path, out container))
            {
                obj = container.GetObject();
            }
            else
            {
                container = containerPool.GetAssetContainer(CalcDisposeTime(path));
                assetContainerMap.Add(path, container);
            }
            if (null == obj)
            {
                obj = AssetLoader.Load(path, type);
                container.MarkObject(obj);
            }
            return obj;
        }

        public GameObject GetGameObject(string path, Transform parent)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetTrackMgr GetGameObject param path is null or empty!");
                return null;
            }

            GameObject gameObject = null;
            GameObjectContainer container;
            if (gameObjectContainerMap.TryGetValue(path, out container))
            {
                gameObject = container.GetObject(parent);
            }
            else
            {
                var prefab = AssetLoader.Load<GameObject>(path);
                if (null != prefab)
                {
                    container = containerPool.GetGameObjectContainer(this, path, prefab, CalcDisposeTime(path), CalcCapcitySize(path));
                    gameObjectContainerMap.Add(path, container);
                    gameObject = container.GetObject(parent);
                }
                else
                {
                    Debug.LogError("AssetTrackMgr GetGameObject load prefab failed!The path is:" + path);
                }
            }
            return gameObject;
        }

        public void Release()
        {
            foreach (var container in assetContainerMap.Values)
            {
                container.Release();
            }
            foreach (var container in gameObjectContainerMap.Values)
            {
                container.Release();
            }
        }

        public void ReleaseGameObject(string path, GameObject gameObject)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetTrackMgr ReleaseGameObject param path is null or empty!");
                return;
            }
            if (null == gameObject)
            {
                Debug.LogError("AssetTrackMgr ReleaseGameObject param Obj is null! " + path);
                return;
            }
            if (false == gameObject.scene.IsValid())
            {
                throw new Exception("AssetTrackMgr ReleaseGameObject param obj is a scene gameoject: " + path);
            }
            GameObjectContainer container;
            if (gameObjectContainerMap.TryGetValue(path, out container))
            {
                container.ReturnObject(gameObject);
            }
            else
            {
                Debug.LogError("AssetTrackMgr ReleaseGameObject param obj is not created by container pool! " + path);
                GameObject.Destroy(gameObject);
            }
        }

        public void ReleaseAsset(string path, Object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetTrackMgr ReleaseAsset param path is null or empty!");
                return;
            }
            if (null == obj)
            {
                Debug.LogError("AssetTrackMgr ReleaseAsset param Obj is null! " + path);
                return;
            }
            if (obj is GameObject)
            {
                var gameobject = obj as GameObject;
                if (false == gameobject.scene.IsValid())
                {
                    throw new Exception("AssetTrackMgr ReleaseAsset param obj is a scene gameoject: " + path);
                }
            }

            AssetContainer container;
            if (assetContainerMap.TryGetValue(path, out container))
            {
                container.MarkObject(obj);
            }
            else
            {
                Debug.LogError("AssetTrackMgr ReleaseAsset param obj is not created by container pool! " + path);
                container = containerPool.GetAssetContainer(CalcDisposeTime(path));
                container.MarkObject(obj);
                assetContainerMap.Add(path, container);
            }
        }

        public void SetCapcitySize(string group, int capcity)
        {
            if (string.IsNullOrEmpty(group))
            {
                G_Capcity = capcity > 0 ? capcity : ILLEGAL_VALUE;
            }
            else
            {
                if (capcity > 0)
                {
                    if (capcityValueMap.ContainsKey(group))
                    {
                        capcityValueMap[group] = capcity;
                    }
                    else
                    {
                        capcityValueMap.Add(group, capcity);
                    }
                }
                else
                {
                    capcityValueMap.Remove(group);
                }
            }

            foreach (var container in gameObjectContainerMap)
            {
                container.Value.Capcaity = CalcCapcitySize(container.Key);
            }
        }

        public void SetDisposeInterval(string group, int disposeTimeInterval)
        {
            if (string.IsNullOrEmpty(group))
            {
                G_DisposeTime = disposeTimeInterval > 0 ? disposeTimeInterval : ILLEGAL_VALUE;
            }
            else
            {
                if (disposeTimeInterval > 0)
                {
                    if (disposeTimeMap.ContainsKey(group))
                    {
                        disposeTimeMap[group] = disposeTimeInterval;
                    }
                    else
                    {
                        disposeTimeMap.Add(group, disposeTimeInterval);
                    }
                }
                else
                {
                    disposeTimeMap.Remove(group);
                }
            }

            foreach (var item in assetContainerMap)
            {
                item.Value.DisposeTime = CalcDisposeTime(item.Key);
            }
            foreach (var item in gameObjectContainerMap)
            {
                item.Value.DisposeTime = CalcDisposeTime(item.Key);
            }
        }
        #endregion

        #region private
        private int CalcCapcitySize(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                foreach (var item in capcityValueMap)
                {
                    if (assetPath.StartsWith(item.Key))
                    {
                        return item.Value;
                    }
                }
            }
            if (G_Capcity > ILLEGAL_VALUE)
            {
                return G_Capcity;
            }
            return CAPCITY_SIZE;
        }

        private int CalcDisposeTime(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                foreach (var item in disposeTimeMap)
                {
                    if (assetPath.StartsWith(item.Key))
                    {
                        return item.Value;
                    }
                }
            }
            if (G_DisposeTime > ILLEGAL_VALUE)
            {
                return G_DisposeTime;
            }
            return DISPOSE_TIME_VALUE;
        }

        private class ContainerMapChecker : MonoBehaviour
        {
            public AssetTrackMgr assetTrackMgr;

            private const float CHECK_INTERVAL = 1f;
            private float lastCheckTime;
            private List<string> removeKeyList = new List<string>();

            // Use this for initialization
            void Start()
            {
                lastCheckTime = Time.realtimeSinceStartup;
            }

            private void CheckAssetContainerMap(float dt)
            {
                removeKeyList.Clear();
                foreach (var element in assetTrackMgr.assetContainerMap)
                {
                    if (false == element.Value.IsAlive(dt))
                    {
                        removeKeyList.Add(element.Key);
                    }
                }
                if (removeKeyList.Count > 0)
                {
                    var assetContainerMap = assetTrackMgr.assetContainerMap;
                    var containerPool = assetTrackMgr.containerPool;
                    foreach (var k in removeKeyList)
                    {
                        var container = assetContainerMap[k];
                        assetContainerMap.Remove(k);
                        containerPool.ReleaseAssetContainer(container);
                    }
                    removeKeyList.Clear();
                }
            }

            private void CheckGameObjectContainerMap(float dt)
            {
                removeKeyList.Clear();
                foreach (var element in assetTrackMgr.gameObjectContainerMap)
                {
                    if (false == element.Value.IsAlive(dt))
                    {
                        removeKeyList.Add(element.Key);
                    }
                }
                if (removeKeyList.Count > 0)
                {
                    var gameObjectContainerMap = assetTrackMgr.gameObjectContainerMap;
                    var containerPool = assetTrackMgr.containerPool;
                    foreach (var k in removeKeyList)
                    {
                        var container = gameObjectContainerMap[k];
                        gameObjectContainerMap.Remove(k);
                        containerPool.ReleaseGameObjectContainer(container);
                    }
                    removeKeyList.Clear();
                }
            }

            // Update is called once per frame
            void Update()
            {
                var dt = Time.realtimeSinceStartup - lastCheckTime;
                if (dt > CHECK_INTERVAL)
                {
                    CheckAssetContainerMap(dt);
                    CheckGameObjectContainerMap(dt);
                    lastCheckTime = Time.realtimeSinceStartup;
                }
            }
        }
        #endregion
    }
}


