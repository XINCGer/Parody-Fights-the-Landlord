//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using Plugins.XAsset;
using ColaFramework.Foundation;
using LitJson;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
#endif

namespace ColaFramework.ToolKit
{
    public enum EnvOption
    {
        CS_DEF_SYMBOL,
        DEVLOPMENT,
        IS_MONO,
        APP_NAME,
        BUILD_PATH,

        MOTHER_PKG,
        HOT_UPDATE_BUILD,

        //CDN相关
        CDN_URL,
        CDN_USERNAME,
        CDN_PASSWORD,
        REMOTE_CDN,

        ANALYZE_BUNDLE,
        DistributionSign,
    }

    /// <summary>
    /// ColaFramework框架的打包脚本
    /// </summary>
    public static class ColaBuildTool
    {
        private const string AppVersionPath = "Assets/Editor/Settings/AppVersion.asset";
        private const string Resource_AppVersionPath = "Assets/Resources/app_version.json";
        private const string AppVersionFileName = "app_version.json";
        private const string Resource_VersionPath = "Assets/Resources/versions.txt";
        private const string CDNVersionControlUrl = "CDN/versioncontrol/{0}/{1}";
        private const string CDNResourceUrl = "CDN/ColaFramework/cdn/{0}/{1}";

        private static Dictionary<EnvOption, string> internalEnvMap = new Dictionary<EnvOption, string>();

        #region 封装打包机BuildPlayer接口

        public static void BuildForAndroid()
        {
            BuildPlayer(BuildTarget.Android);
        }

        public static void BuildForiOS()
        {
            BuildPlayer(BuildTarget.iOS);
        }

        #endregion

        #region BuildPlayer接口
        public static void BuildPlayer(BuildTarget buildTarget)
        {
            //检查BuildSceneList
            if (!CheckScenesInBuildValid())
            {
                return;
            }

            //切换平台
            if (buildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                Debug.Log("Start switch platform to: " + buildTarget);
                var beginTime = System.DateTime.Now;
                var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);
                Debug.Log("End switch platform to: " + buildTarget);
                Debug.Log("=================Build SwitchPlatform Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
            }

            //0.根据buildTarget区分BuildGroup
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            if (BuildTargetGroup.Unknown == buildTargetGroup)
            {
                throw new System.Exception(string.Format("{0} is Unknown Build Platform ! Build Failture!", buildTarget));
            }
            try
            {
                //1.首先确认各种环境变量和配置到位
                InitBuildEnvironment(buildTargetGroup);

                //2.自动化接入SDK
                BuildSDK(buildTargetGroup);

                //3.设置参数
                SetBuildParams(buildTargetGroup);

                //4.处理Lua文件
                BuildLua(buildTargetGroup);

                //5.打Bundle
                BuildAssetBundle(buildTargetGroup);

                //6.自动处理AppVersion
                BuildAppVersion();

                //7.UpLoadCDN
                UpLoadCDN(buildTargetGroup);

                //8.出包
                InternalBuildPkg(buildTargetGroup);
            }
            catch
            {
                throw;
            }
            finally
            {
                //8.清理工作，恢复工作区
                CleanUp(buildTargetGroup);
            }
        }

        /// <summary>
        /// 初始化各种基本的路径和SDK、JDK等必要的打包配置环境
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void InitBuildEnvironment(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 处理非侵入式SDK的接入(Android端)
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildSDK(BuildTargetGroup buildTargetGroup)
        {

        }

#if UNITY_IOS
        private const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";

        /// <summary>
        /// 处理非侵入式SDK的接入(iOS端)
        /// </summary>
        [PostProcessBuild]
        public static void OnPostBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            string pbxProjPath = PBXProject.GetPBXProjectPath(buildPath);
            var pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxProjPath));

#if UNITY_2019_3_OR_NEWER
            string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
#else
            string targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
            var projRoot = Path.GetDirectoryName(Application.dataPath);

            //分解为多个步骤来配置iOS的工程，在每一步根据对应的Option，进行对应的操作

            // 1.设置关闭Bitcode（如果不需要，可注释掉）
            pbxProject.SetBuildProperty(targetGuid, ENABLE_BITCODE_KEY, "NO");
            //.重新写回配置
            File.WriteAllText(pbxProjPath, pbxProject.WriteToString());
        }
#endif

        /// <summary>
        /// 用来设置一些编译的宏和参数等操作
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void SetBuildParams(BuildTargetGroup buildTargetGroup)
        {
            //Android包在这里做签名操作
            var isDistributionSign = ContainsEnvOption(EnvOption.DistributionSign);
            if (!isDistributionSign)
            {
                PlayerSettings.Android.keystoreName = "";
                PlayerSettings.Android.keystorePass = "";
                PlayerSettings.Android.keyaliasName = "";
                PlayerSettings.Android.keyaliasPass = "";
            }
            else
            {
                PlayerSettings.Android.keystoreName = "./Tools/user.keystore";
                PlayerSettings.Android.keystorePass = "password";
                PlayerSettings.Android.keyaliasName = "cola";
                PlayerSettings.Android.keyaliasPass = "password";
            }

            PlayerSettings.productName = GetEnvironmentVariable(EnvOption.APP_NAME);
            var CS_DefineSymbol = GetEnvironmentVariable(EnvOption.CS_DEF_SYMBOL);
            if (!string.IsNullOrEmpty(CS_DefineSymbol))
            {
                var oldSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                oldSymbol = oldSymbol + ";" + oldSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, oldSymbol);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 清理与恢复工作区
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void CleanUp(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 最后的出包环节
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void InternalBuildPkg(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            if (!ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD))
            {
                var outputPath = GetEnvironmentVariable(EnvOption.BUILD_PATH);
                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
                }
                if (outputPath.Length == 0)
                    return;

                var levels = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
                if (levels.Length == 0)
                {
                    Debug.Log("Nothing to build.");
                    return;
                }

                var targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
                if (targetName == null)
                    return;
                var buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = levels,
                    locationPathName = outputPath + targetName,
                    assetBundleManifestPath = GetAssetBundleManifestFilePath(),
                    target = EditorUserBuildSettings.activeBuildTarget,
                };
                if (ContainsEnvOption(EnvOption.DEVLOPMENT))
                {
                    buildPlayerOptions.options |= BuildOptions.Development;
                    buildPlayerOptions.options |= BuildOptions.AllowDebugging;
                    buildPlayerOptions.options |= BuildOptions.ConnectWithProfiler;
                }
                if (ContainsEnvOption(EnvOption.IS_MONO))
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                }

                AssetDatabase.SaveAssets();
                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
            Debug.Log("=================Build Pkg Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// Build AssetBundle
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildAssetBundle(BuildTargetGroup buildTargetGroup)
        {
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);

            var beginTime = System.DateTime.Now;
            if (!isHotUpdateBuild || (isHotUpdateBuild && ContainsEnvOption(EnvOption.ANALYZE_BUNDLE)))
            {
                AssetBundleAnalyzer.AutoAnalyzeAssetBundleName();
            }
            Debug.Log("=================Build AutoAnalyzeAssetBundleName Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);

            beginTime = System.DateTime.Now;
            ColaEditHelper.BuildManifest();
            ColaEditHelper.BuildAssetBundles();

            if (!isHotUpdateBuild)
            {
                ColaEditHelper.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles));
                AssetDatabase.Refresh();

                BuildVideoFiles();
            }
            Debug.Log("=================Build BuildAssetBundle Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// 处理视频文件
        /// </summary>
        private static void BuildVideoFiles()
        {
            FileHelper.CopyDir("Assets/RawAssets/Videos/", Application.streamingAssetsPath + "/Videos/");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Build Lua
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildLua(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);
            if (AppConst.LuaBundleMode)
            {
                ColaEditHelper.BuildLuaBundle(isMotherPkg, isHotUpdateBuild);
            }
            else
            {
                ColaEditHelper.BuildLuaFile(isMotherPkg, isHotUpdateBuild);
            }
            Debug.Log("=================Build Lua Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// UpLoadCDN
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void UpLoadCDN(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);

            if (isHotUpdateBuild || isMotherPkg)
            {
                //上传远端CDN
                if (ContainsEnvOption(EnvOption.REMOTE_CDN))
                {
                    var host = GetEnvironmentVariable(EnvOption.CDN_URL);
                    var userName = GetEnvironmentVariable(EnvOption.CDN_USERNAME);
                    var password = GetEnvironmentVariable(EnvOption.CDN_PASSWORD);
                    var ftpUtil = new FTPUtil(host, userName, password);

                    //复制到本地目录以后打成zip，然后把zip上传到CDN
                    //upload appversion.json
                    var tempCDNRoot = "ColaCache/CDN";
                    FileHelper.RmDir(tempCDNRoot);

                    var cachePath = ColaEditHelper.TempCachePath + "/" + AppVersionFileName;
                    var CDN_AppVersionPath = string.Format(ColaEditHelper.TempCachePath + "/" + CDNVersionControlUrl, ColaEditHelper.GetPlatformName(), "app_version.json");
                    FileHelper.CopyFile(cachePath, CDN_AppVersionPath, true);

                    var CDN_ResourcePath = string.Format(ColaEditHelper.TempCachePath + "/" + CDNResourceUrl, ColaEditHelper.GetPlatformName(), PlayerSettings.bundleVersion);
                    //upload version.txt and assets
                    var reltaRoot = ColaEditHelper.CreateAssetBundleDirectory();
                    var updateFilePath = reltaRoot + "/updates.txt";
                    using (var sr = new StreamReader(updateFilePath))
                    {
                        var content = sr.ReadLine();
                        while (null != content)
                        {
                            var reltaPath = reltaRoot + "/" + content;
                            var destPath = CDN_ResourcePath + "/" + content;
                            FileHelper.CopyFile(reltaPath, destPath, true);
                            content = sr.ReadLine();
                        }
                    }
                    FileHelper.CopyFile(reltaRoot + "/versions.txt", ColaEditHelper.TempCachePath + "/" + CDNResourceUrl + "versions.txt", true);

                    //ZIP
                    var zipPath = "ColaCache/hotupdate.zip";
                    FileHelper.DeleteFile(zipPath);
                    var result = ZipHelper.Zip(tempCDNRoot, zipPath);
                    if (result)
                    {
                        ftpUtil.Upload(zipPath, "Upload/hotupdate.zip");
                    }
                    else
                    {
                        Debug.LogError("Zip并上传CDN过程中出现错误！");
                    }
                }
                //上传本地CDN，打包机和CDN是同一台机器
                else
                {
                    //upload appversion.json
                    var cachePath = ColaEditHelper.TempCachePath + "/" + AppVersionFileName;
                    var CDN_AppVersionPath = string.Format(CDNVersionControlUrl, ColaEditHelper.GetPlatformName(), "app_version.json");
                    FileHelper.CopyFile(cachePath, CDN_AppVersionPath, true);

                    //upload version.txt and assets
                    var reltaRoot = ColaEditHelper.CreateAssetBundleDirectory();
                    var updateFilePath = reltaRoot + "/updates.txt";
                    var CDN_ResourcePath = string.Format(CDNResourceUrl, ColaEditHelper.GetPlatformName(), PlayerSettings.bundleVersion);
                    using (var sr = new StreamReader(updateFilePath))
                    {
                        var content = sr.ReadLine();
                        while (null != content)
                        {
                            var reltaPath = reltaRoot + "/" + content;
                            var destPath = CDN_ResourcePath + "/" + content;
                            FileHelper.CopyFile(reltaPath, destPath, true);
                            content = sr.ReadLine();
                        }
                    }
                    FileHelper.CopyFile(reltaRoot + "/versions.txt", CDN_ResourcePath + "/" + "versions.txt", true);
                }

            }
            Debug.Log("=================UpLoadCDN Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// 自动处理AppVersion
        /// </summary>
        private static void BuildAppVersion()
        {
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);

            var appAsset = ColaEditHelper.GetScriptableObjectAsset<AppVersion>(AppVersionPath);
            if (isHotUpdateBuild)
            {
                appAsset.HotUpdateVersion += 1;
            }
            else if (isMotherPkg)
            {
                appAsset.HotUpdateVersion = 0;
                appAsset.StoreVersion += 1;
                appAsset.BuildVersion = 0;
            }
            if (!isMotherPkg && !isHotUpdateBuild)
            {
                appAsset.BuildVersion += 1;
            }
            appAsset.OnValueChanged();
            EditorUtility.SetDirty(appAsset);
            AssetDatabase.SaveAssets();

            if (null != appAsset)
            {
                var jsonStr = JsonMapper.ToJson(appAsset);
                var cachePath = ColaEditHelper.TempCachePath + "/" + AppVersionFileName;
                FileHelper.DeleteFile(cachePath);
                FileHelper.WriteString(cachePath, jsonStr);
                if (!isHotUpdateBuild)
                {
                    FileHelper.CopyFile(cachePath, Resource_AppVersionPath, true);

                    PlayerSettings.bundleVersion = appAsset.Version;
                }
            }

            if (!isHotUpdateBuild)
            {
                FileHelper.DeleteFile(Resource_VersionPath);
                var outputPath = ColaEditHelper.CreateAssetBundleDirectory();
                var versionsTxt = outputPath + "/versions.txt";
                FileHelper.CopyFile(versionsTxt, Resource_VersionPath, true);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 对打包平台进行分组
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        private static BuildTargetGroup HandleBuildGroup(BuildTarget buildTarget)
        {
            var buildTargetGroup = BuildTargetGroup.Unknown;
            if (BuildTarget.Android == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Android;
            }
            else if (BuildTarget.iOS == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.iOS;
            }
            else if (BuildTarget.StandaloneWindows == buildTarget || BuildTarget.StandaloneWindows64 == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            return buildTargetGroup;
        }

        private static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "/" + PlayerSettings.productName + ".apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/" + PlayerSettings.productName + ".exe";
                case BuildTarget.iOS:
                    return "";
                // Add more build targets for your own.
                default:
                    Debug.LogError("Build Target not implemented.");
                    return null;
            }
        }

        private static string GetAssetBundleManifestFilePath()
        {
            var relativeAssetBundlesOutputPathForPlatform = Path.Combine(Utility.AssetBundles, ColaEditHelper.GetPlatformName());
            return Path.Combine(relativeAssetBundlesOutputPathForPlatform, ColaEditHelper.GetPlatformName()) + ".manifest";
        }

        public static void EnsureParentDirExist(FTPUtil ftpUtil, string path)
        {
            var dir = Path.GetDirectoryName(path);
            var parents = new Queue<string>();
            while (!string.IsNullOrEmpty(dir))
            {
                parents.Enqueue(dir);
                dir = Path.GetDirectoryName(dir);
            }
            while (parents.Count > 0)
            {
                if (null != ftpUtil)
                {
                    try
                    {
                        ftpUtil.MakeDirectory(parents.Dequeue());
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    Debug.LogError("EnsureParentDirExist throw exception! FTPUtil is null!");
                }

            }
        }
        #endregion

        #region 工具方法
        public static bool CheckScenesInBuildValid()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!File.Exists(scene.path))
                {
                    Debug.LogError("Error! Scene In BuildList中有场景丢失！请检查！");
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 环境变量
        public static string GetEnvironmentVariable(EnvOption envOption)
        {
            return internalEnvMap.ContainsKey(envOption) ? internalEnvMap[envOption] : Environment.GetEnvironmentVariable(envOption.ToString()) ?? string.Empty;
        }

        public static void SetEnvironmentVariable(EnvOption envOption, string value, bool isAppend)
        {
            string oldValue = GetEnvironmentVariable(envOption);
            if (!isAppend)
            {
                oldValue = value;
            }
            else
            {
                oldValue = string.IsNullOrEmpty(oldValue) ? value : (oldValue + ";" + value);
            }
            if (!internalEnvMap.ContainsKey(envOption))
            {
                internalEnvMap.Add(envOption, oldValue);
            }
            else
            {
                internalEnvMap[envOption] = oldValue;
            }
        }

        public static bool ContainsEnvOption(EnvOption envOption)
        {
            string envVar = GetEnvironmentVariable(envOption);
            if (string.IsNullOrEmpty(envVar) || 0 == string.Compare(envVar, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static void ClearEnvironmentVariable()
        {
            internalEnvMap.Clear();
        }
        #endregion
    }
}