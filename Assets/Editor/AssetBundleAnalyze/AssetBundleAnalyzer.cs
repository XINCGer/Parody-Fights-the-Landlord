//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------


using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace ColaFramework.ToolKit
{
    public class AssetBundleAnalyzer
    {
        private const string BUILD_RULES_PATH = "Assets/Editor/Settings/BuildRules.asset";
        private static Dictionary<string, AssetInfo> assetInfoDict = new Dictionary<string, AssetInfo>();
        private const int PIECE_THRESHOLD = 2;  //颗粒度，当一个资源大于这个数目被引用时，会抽取为单独的公共Bundle
        private static string curRootAsset = string.Empty;
        private static float curProgress = 0f;
        private static BuildRules buildRules;


        [MenuItem("Build/AssetBundleAnalyzer/AnalyzeAssetbundleName")]
        static void SetABNames()
        {
            string path = GetSelectedAssetPath();
            if (path == null)
            {
                Debug.LogWarning("请先选择目标文件夹");
                return;
            }
            GetAllAssets(path);

        }
        [MenuItem("Build/AssetBundleAnalyzer/ClearAllAssetbundelname")]
        static void ClearAllABNames()
        {
            ColaEditHelper.ClearAllAssetBundleName();
        }

        public static void AutoAnalyzeAssetBundleName()
        {
            GetAllAssets(Constants.GameAssetBasePath);

            //按文件夹标记
            var markDirList = buildRules.MarkWithDirList;
            if (null != markDirList)
            {
                foreach (var item in markDirList)
                {
                    ColaEditHelper.MarkAssetsWithDir(item);
                }
            }

            //标记为一个bundle
            var markOneBundleList = buildRules.MarkWithOneBundleList;
            if (null != markOneBundleList)
            {
                foreach (var item in markOneBundleList)
                {
                    var bundleName = item.TrimEnd('/');
                    var index = bundleName.LastIndexOf('/');
                    bundleName = bundleName.Substring(index);
                    bundleName = bundleName.TrimStart('/');
                    bundleName += AppConst.ExtName;
                    ColaEditHelper.MarkAssetsToOneBundle(item, bundleName);
                }
            }

            //按文件标记
            var markFileList = buildRules.MarkWithFileList;
            if (null != markFileList)
            {
                foreach (var item in markFileList)
                {
                    ColaEditHelper.MarkAssetsWithFile(item);
                }
            }
        }

        public static void GetAllAssets(string rootDir)
        {
            assetInfoDict.Clear();
            DirectoryInfo dirinfo = new DirectoryInfo(rootDir);
            FileInfo[] fs = dirinfo.GetFiles("*.*", SearchOption.AllDirectories);
            int ind = 0;
            buildRules = ColaEditHelper.GetScriptableObjectAsset<BuildRules>(BUILD_RULES_PATH);
            foreach (var f in fs)
            {
                curProgress = (float)ind / (float)fs.Length;
                curRootAsset = "正在分析依赖：" + f.Name;
                EditorUtility.DisplayProgressBar(curRootAsset, curRootAsset, curProgress);
                ind++;
                int index = f.FullName.IndexOf("Assets");
                if (index != -1)
                {
                    string assetPath = f.FullName.Substring(index);
                    if (assetPath.EndsWith(".meta", StringComparison.Ordinal) || assetPath.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    {
                        continue;
                    }
                    UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    string upath = AssetDatabase.GetAssetPath(asset);
                    if (buildRules.IsInBuildRules(upath))
                    {
                        EditorUtility.UnloadUnusedAssetsImmediate();
                        continue;
                    }
                    if (assetInfoDict.ContainsKey(assetPath) == false
                        && assetPath.StartsWith("Assets")
                        && !(asset is LightingDataAsset)
                        && asset != null
                        )
                    {
                        AssetInfo info = new AssetInfo(upath, true);
                        //标记一下是文件夹下根资源
                        CreateDeps(info);
                    }
                    EditorUtility.UnloadUnusedAssetsImmediate();
                }
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            EditorUtility.ClearProgressBar();

            int setIndex = 0;
            foreach (KeyValuePair<string, AssetInfo> kv in assetInfoDict)
            {
                EditorUtility.DisplayProgressBar("正在设置AssetBundleName: ", kv.Key, (float)setIndex / (float)assetInfoDict.Count);
                setIndex++;
                AssetInfo a = kv.Value;
                a.SetAssetBundleName(PIECE_THRESHOLD);
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.UnloadUnusedAssetsImmediate();
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 递归分析每个所被依赖到的资源
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parent"></param>
        static void CreateDeps(AssetInfo self, AssetInfo parent = null)
        {
            if (self.HasParent(parent))
                return;
            if (assetInfoDict.ContainsKey(self.assetPath) == false)
            {
                assetInfoDict.Add(self.assetPath, self);
            }
            self.AddParent(parent);

            UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { self.GetAsset() });
            for (int i = 0; i < deps.Length; i++)
            {
                UnityEngine.Object o = deps[i];
                if (o is MonoScript || o is LightingDataAsset)
                    continue;
                string path = AssetDatabase.GetAssetPath(o);
                if (path == self.assetPath)
                    continue;
                if (path.StartsWith("Assets") == false)
                    continue;
                if (buildRules.IsInBuildRules(path))
                {
                    continue;
                }
                AssetInfo info = null;
                if (assetInfoDict.ContainsKey(path))
                {
                    info = assetInfoDict[path];
                }
                else
                {
                    info = new AssetInfo(path);
                    assetInfoDict.Add(path, info);
                }
                EditorUtility.DisplayProgressBar(curRootAsset, path, curProgress);
                CreateDeps(info, self);
            }
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        static string GetSelectedAssetPath()
        {
            var selected = Selection.activeObject;
            if (selected == null)
            {
                return null;
            }
            Debug.Log(selected.GetType());
            if (selected is DefaultAsset)
            {
                string path = AssetDatabase.GetAssetPath(selected);
                Debug.Log("选中路径： " + path);
                return path;
            }
            else
            {
                return null;
            }
        }
    }
}
