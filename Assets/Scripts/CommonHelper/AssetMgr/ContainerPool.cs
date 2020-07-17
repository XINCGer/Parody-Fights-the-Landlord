//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework.Foundation
{
    public class ContainerPool
    {
        private ObjectPool<AssetContainer> assetContainerPool = new ObjectPool<AssetContainer>(CreateAssetContainer, null, AssetContainer.Dispose);
        private ObjectPool<GameObjectContainer> gameobjectContainerPool = new ObjectPool<GameObjectContainer>(CreateGameObjectContainer, null, GameObjectContainer.Dispose);

        #region get && release
        public AssetContainer GetAssetContainer(int disposeTime)
        {
            var container = assetContainerPool.Get();
            return AssetContainer.Process(container, disposeTime);
        }

        public void ReleaseAssetContainer(AssetContainer container)
        {
            assetContainerPool.Release(container);
        }

        public GameObjectContainer GetGameObjectContainer(AssetTrackMgr assetTrackMgr, string path, GameObject prefab, int disposeTime, int capcity)
        {
            var container = gameobjectContainerPool.Get();
            return GameObjectContainer.Process(assetTrackMgr, container, path, prefab, disposeTime, capcity);
        }

        public void ReleaseGameObjectContainer(GameObjectContainer container)
        {
            gameobjectContainerPool.Release(container);
        }
        #endregion

        #region CreateContainer
        private static AssetContainer CreateAssetContainer()
        {
            return new AssetContainer();
        }

        private static GameObjectContainer CreateGameObjectContainer()
        {
            return new GameObjectContainer();
        }
        #endregion
    }

    #region Container implement
    public class AssetContainer
    {
        private Object hardRef;
        private System.WeakReference weakRef;
        private float disposeTimeTicker;

        public int DisposeTime { get; set; }

        public static AssetContainer Process(AssetContainer container, int disposeTime)
        {
            container.DisposeTime = disposeTime;
            return container;
        }

        public bool IsAlive(float dt)
        {
            if (null != hardRef)
            {
                if (disposeTimeTicker > 0)
                {
                    disposeTimeTicker -= dt;
                }
                else
                {
                    if (null == weakRef)
                    {
                        weakRef = new System.WeakReference(hardRef);
                    }
                    else
                    {
                        weakRef.Target = hardRef;
                    }
                    hardRef = null;
                }
                return true;
            }
            else
            {
                return null != weakRef && null != weakRef.Target;
            }
        }

        public Object GetObject()
        {
            Object obj = null;
            if (null != hardRef)
            {
                obj = hardRef;
            }
            else
            {
                if (null != weakRef)
                {
                    hardRef = weakRef.Target as Object;
                    weakRef.Target = null;
                    obj = hardRef;
                }
                else
                {
                    Debug.LogWarning("the asset has been dispose but not be removed from container pool");
                }
            }
            disposeTimeTicker = DisposeTime;
            return obj;
        }

        public void MarkObject(Object target)
        {
            if (null == hardRef)
            {
                hardRef = target;
            }
            else
            {
                Debug.AssertFormat(target == hardRef, "hard ref:{0},target:{1},the container is repeat contain asset!", hardRef.name, target.name);
            }
            if (null != weakRef)
            {
                weakRef.Target = null;
            }
            disposeTimeTicker = DisposeTime;
        }

        public void Release()
        {
            hardRef = null;
            disposeTimeTicker = 0;
        }

        public static void Dispose(AssetContainer container)
        {
            container.Release();
            if (null != container.weakRef)
            {
                container.weakRef.Target = null;
            }
            container.DisposeTime = AssetTrackMgr.DISPOSE_TIME_VALUE;
        }
    }

    public class GameObjectContainer
    {
        private GameObject prefab;
        private AssetTrackMgr assetTrackMgr;
        private int containerSize;
        private string path;
        private float disposeTimeTicker;
        private float sleepTimerTicker;
        private int refCount;

        private LinkedList<GameObject> objectList = new LinkedList<GameObject>();

        public int DisposeTime { get; set; }
        public int Capcaity { get; set; }

        public static GameObjectContainer Process(AssetTrackMgr assetTrackMgr, GameObjectContainer container, string path, GameObject prefab, int disposeTime, int capcity)
        {
            container.assetTrackMgr = assetTrackMgr;
            container.path = path;
            container.prefab = prefab;
            container.DisposeTime = disposeTime;
            container.Capcaity = capcity;
            return container;
        }

        public bool IsAlive(float dt)
        {
            if (sleepTimerTicker > 0)
            {
                sleepTimerTicker -= dt;
                return true;
            }
            if (objectList.Count > 0)
            {
                if (disposeTimeTicker > 0)
                {
                    disposeTimeTicker -= dt;
                }
                else
                {
                    var gameobject = objectList.Last.Value;
                    objectList.RemoveLast();
                    Discard(gameobject);
                }
                return true;
            }
            else if (refCount > 0)
            {
                return true;
            }
            else
            {
                if (null != prefab)
                {
                    if (disposeTimeTicker > 0)
                    {
                        disposeTimeTicker -= dt;
                        return true;
                    }
                    else
                    {
                        prefab = null;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public GameObject GetObject(Transform parent)
        {
            sleepTimerTicker = DisposeTime;
            disposeTimeTicker = AssetTrackMgr.DISPOSE_CHECK_INTERVAL;

            if (objectList.Count > 0)
            {
                var gameObject = objectList.Last.Value;
                objectList.RemoveLast();
                gameObject.SetActive(true);
                if (null != parent)
                    gameObject.transform.SetParent(parent, false);
                return gameObject;
            }
            else
            {
                var gameObject = assetTrackMgr.instantiateAction(prefab) as GameObject;
                //gameObject.name = path;
                gameObject.name = prefab.name;
                if (null != parent)
                    gameObject.transform.SetParent(parent, false);
                gameObject.transform.localPosition = prefab.transform.localPosition;
                gameObject.transform.localRotation = prefab.transform.localRotation;
                gameObject.transform.localScale = prefab.transform.localScale;
                refCount++;
                return gameObject;
            }
        }

        public void ReturnObject(GameObject gameObject)
        {
            if (null == gameObject) return;
            if (objectList.Count < Capcaity)
            {
                sleepTimerTicker = DisposeTime;
                disposeTimeTicker = AssetTrackMgr.DISPOSE_CHECK_INTERVAL;

                //gameObject.name = path;
                gameObject.transform.SetParent(assetTrackMgr.rootTF, false);
                gameObject.SetActive(false);
                objectList.AddFirst(gameObject);
            }
            else
            {
                Discard(gameObject);
            }
        }

        public void Discard(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
            refCount--;
            disposeTimeTicker = refCount < 0 ? DisposeTime : AssetTrackMgr.DISPOSE_CHECK_INTERVAL;
        }

        public void Release()
        {
            foreach (var gameObject in objectList)
            {
                GameObject.Destroy(gameObject);
                refCount--;
            }
            objectList.Clear();
            sleepTimerTicker = 0;
            disposeTimeTicker = 0;
        }

        public static void Dispose(GameObjectContainer container)
        {
            container.Release();
            container.path = null;
            container.prefab = null;
            container.assetTrackMgr = null;
            container.refCount = 0;
            container.Capcaity = AssetTrackMgr.CAPCITY_SIZE;
            container.DisposeTime = AssetTrackMgr.DISPOSE_TIME_VALUE;
        }
    }
    #endregion
}


