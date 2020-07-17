//
// Utility.cs
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

using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.XAsset
{
    public delegate Object LoadDelegate(string path, Type type);

    public delegate string GetPlatformDelegate();

    public static class Utility
    {
        public const string AssetBundles = "AssetBundles";
        public const string AssetsManifestAsset = "Assets/GameAssets/Manifest.asset";
        public const string ManifestAsset = "Manifest.asset";
        public static bool assetBundleMode = true;
        public static LoadDelegate loadDelegate = null;
        public static GetPlatformDelegate getPlatformDelegate = null;

        public static string dataPath { get; set; }
        public static string downloadURL { get; set; }

        public static string GetPlatform()
        {
            return getPlatformDelegate != null
                ? getPlatformDelegate()
                : GetPlatformForAssetBundles(Application.platform);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }

        public static string UpdatePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, AssetBundles) +
                       Path.DirectorySeparatorChar;
            }
        }

        public static string GetRelativePath4Update(string path)
        {
            return UpdatePath + path;
        }

        public static string GetDownloadURL(string filename)
        {
            return Path.Combine(Path.Combine(downloadURL, GetPlatform()), filename);
        }

        public static string GetWebUrlFromDataPath(string filename)
        {
            var path = Path.Combine(dataPath, AssetBundles) + Path.DirectorySeparatorChar + filename;
#if UNITY_IOS || UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            path = "file:///" + path;
#endif
            return path;
        }

        public static string GetWebUrlFromStreamingAssets(string filename)
        {
            var path = UpdatePath + filename;
            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/" + filename;
            }
#if UNITY_IOS || UNITY_EDITOR
			path = "file://" + path;
#elif UNITY_STANDALONE_WIN
            path = "file:///" + path;
#endif
            return path;
        }
    }
}