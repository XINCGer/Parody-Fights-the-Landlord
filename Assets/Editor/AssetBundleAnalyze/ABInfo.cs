//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------


using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace ColaFramework.ToolKit
{
    public class AssetInfo
    {
        //是不是被打包文件夹下的直接资源
        private bool isRootAsset = false;

        public string assetPath { get; private set; }

        private HashSet<AssetInfo> childSet = new HashSet<AssetInfo>();
        private HashSet<AssetInfo> parentSet = new HashSet<AssetInfo>();

        public AssetInfo(string assetPath, bool isRootAsset = false)
        {
            this.assetPath = assetPath;
        }
        public Object GetAsset()
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            return asset;
        }
        /// <summary>
        /// 从这里开始分析构建资源依赖树
        /// </summary>
        /// <param name="parent"></param>
        public void AddParent(AssetInfo parent)
        {
            if (parent == this || IsParentEarlyDep(parent) || parent == null)
                return;

            parentSet.Add(parent);
            parent.AddChild(this);

            parent.RemoveRepeatChildDep(this);
            RemoveRepeatParentDep(parent);
        }

        /// <summary>
        /// 清除我父节点对我子节点的重复引用，保证树形结构
        /// </summary>
        /// <param name="targetParent"></param>
        private void RemoveRepeatChildDep(AssetInfo targetChild)
        {

            List<AssetInfo> infolist = new List<AssetInfo>(parentSet);
            for (int i = 0; i < infolist.Count; i++)
            {
                AssetInfo pinfo = infolist[i];
                pinfo.RemoveChild(targetChild);
                pinfo.RemoveRepeatChildDep(targetChild);
            }
        }
        /// <summary>
        /// 清除我子节点被我父节点的重复引用，保证树形结构
        /// </summary>
        /// <param name="targetChild"></param>
        private void RemoveRepeatParentDep(AssetInfo targetParent)
        {

            List<AssetInfo> infolist = new List<AssetInfo>(childSet);
            for (int i = 0; i < infolist.Count; i++)
            {
                AssetInfo cinfo = infolist[i];
                cinfo.RemoveParent(targetParent);
                cinfo.RemoveRepeatParentDep(targetParent);
            }
        }

        private void RemoveChild(AssetInfo targetChild)
        {
            childSet.Remove(targetChild);
            targetChild.parentSet.Remove(this);
        }

        private void RemoveParent(AssetInfo parent)
        {
            parent.childSet.Remove(this);
            parentSet.Remove(parent);
        }

        private void AddChild(AssetInfo child)
        {
            childSet.Add(child);
        }

        /// <summary>
        /// 如果父节点早已当此父节点为父节点
        /// </summary>
        /// <param name="targetParent"></param>
        /// <returns></returns>
        private bool IsParentEarlyDep(AssetInfo targetParent)
        {
            if (parentSet.Contains(targetParent))
            {
                return true;
            }
            var e = parentSet.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.IsParentEarlyDep(targetParent))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasParent(AssetInfo p)
        {
            if (parentSet.Contains(p))
                return true;
            return false;
        }

        /// <summary>
        /// 按打包碎片粒SetAssetBundleName
        /// </summary>
        /// <param name="pieceThreshold"></param>
        public void SetAssetBundleName(int pieceThreshold)
        {
            var abName = TrimedAssetBundleName(assetPath) + AppConst.ExtName;
            //大于阀值
            if (this.parentSet.Count >= pieceThreshold)
            {
                ColaEditHelper.SetAssetBundleNameAndVariant(assetPath, abName, null);
                //Debug.Log("<color=#6501AB>" + "设置ab，有多个引用: " + this.assetPath + "</color>");
            }
            //根节点
            else if (this.parentSet.Count == 0 || this.isRootAsset)
            {
                ColaEditHelper.SetAssetBundleNameAndVariant(assetPath, abName, null);
                //Debug.Log("<color=#025082>" + "设置ab，根资源ab: " + this.assetPath + "</color>");
            }
            else
            {
                //其余的子资源
                ColaEditHelper.SetAssetBundleNameAndVariant(assetPath, string.Empty, null);
                //Debug.Log("<color=#DBAF00>" + "清除ab， 仅有1个引用: " + this.assetPath + "</color>");
            }
        }

        private string TrimedAssetBundleName(string assetBundleName)
        {
            assetBundleName = assetBundleName.Replace(Constants.GameAssetBasePath, "");
            assetBundleName = assetBundleName.Replace(Constants.AssetRoot, "");
            return assetBundleName.ToLower();
        }
    }
}


