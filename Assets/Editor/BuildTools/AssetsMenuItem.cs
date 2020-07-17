//
// AssetsMenuItem.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2019 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.IO;
using UnityEditor;
using UnityEngine;
using ColaFramework;
using ColaFramework.ToolKit;
using ColaFramework.Foundation;

namespace Plugins.XAsset.Editor
{
    public static class AssetsMenuItem
    {
        private const string MARK_ASSET_WITH_DIR = "Assets/AssetBundles/按目录标记";
        private const string MARK_ASSET_WITH_FILE = "Assets/AssetBundles/按文件标记";
        private const string MARK_ASSET_WITH_NAME = "Assets/AssetBundles/按名称标记";
        private const string CLEAR_ABNAME = "Assets/AssetBundles/清除所有AssetBundle的标记";
        private const string BUILD_MANIFEST = "Assets/AssetBundles/生成配置";
        private const string BUILD_ASSETBUNDLES = "Assets/AssetBundles/生成资源包";
        private const string MARK_ASSETS = "正在标记资源";
        private const string CLEAR_SANDBOX = "Assets/AssetBundles/清除沙盒目录下的内容";

        [InitializeOnLoadMethod]
        public static void OnInitialize()
        {
            EditorUtility.ClearProgressBar();
            Utility.dataPath = System.Environment.CurrentDirectory;
            Utility.downloadURL = ColaEditHelper.GetManifest().downloadURL;
            Utility.assetBundleMode = AppConst.SimulateMode;
            Utility.getPlatformDelegate = ColaEditHelper.GetPlatformName;
            Utility.loadDelegate = AssetDatabase.LoadAssetAtPath;
        }

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            return assetBundleName.Replace(Constants.GameAssetBasePath, "");
        }

        [MenuItem(MARK_ASSET_WITH_DIR)]
        private static void MarkAssetsWithDir()
        {
            var assetsManifest = ColaEditHelper.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(MARK_ASSETS, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(path).Replace("\\", "/")) + "_g";
                ColaEditHelper.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(MARK_ASSET_WITH_FILE)]
        private static void MarkAssetsWithFile()
        {
            var assetsManifest = ColaEditHelper.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(MARK_ASSETS, path, i * 1f / assets.Length))
                    break;

                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                if (dir == null)
                    continue;
                dir = dir.Replace("\\", "/") + "/";
                if (name == null)
                    continue;

                var assetBundleName = TrimedAssetBundleName(Path.Combine(dir, name));
                ColaEditHelper.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(MARK_ASSET_WITH_NAME)]
        private static void MarkAssetsWithName()
        {
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var assetsManifest = ColaEditHelper.GetManifest();
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(MARK_ASSETS, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = Path.GetFileNameWithoutExtension(path);
                ColaEditHelper.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(CLEAR_ABNAME)]
        private static void ClearAllABName()
        {
            ColaEditHelper.ClearAllAssetBundleName();
        }

        [MenuItem(BUILD_MANIFEST)]
        private static void BuildManifest()
        {
            ColaEditHelper.BuildManifest();
        }

        [MenuItem(BUILD_ASSETBUNDLES)]
        private static void BuildAssetBundles()
        {
            ColaEditHelper.BuildManifest();
            ColaEditHelper.BuildAssetBundles();
        }

        [MenuItem(CLEAR_SANDBOX)]
        private static void ClearSandBox()
        {
            FileHelper.RmDir(Utility.UpdatePath);
            FileHelper.RmDir(AppConst.DataPath);
            FileHelper.RmDir(AppConst.UpdateCachePath);
        }
    }
}