//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ColaFramework.Foundation;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Plugins.XAsset;
using System.Text;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// ColaFramework 编辑器助手类
    /// </summary>
    public class ColaEditHelper
    {
        public static string overloadedDevelopmentServerURL = "";
        public static string m_projectRoot;
        public static string m_projectRootWithSplit;

        /// <summary>
        /// 编辑器会用到的一些临时目录
        /// </summary>
        public static string TempCachePath
        {
            get { return Path.Combine(Application.dataPath, "../ColaCache"); }
        }

        public static string ProjectRoot
        {
            get
            {
                if (string.IsNullOrEmpty(m_projectRoot))
                {
                    m_projectRoot = FileHelper.FormatPath(Path.GetDirectoryName(Application.dataPath));
                }

                return m_projectRoot;
            }
        }

        public static string ProjectRootWithSplit
        {
            get
            {
                if (string.IsNullOrEmpty(m_projectRootWithSplit))
                {
                    m_projectRootWithSplit = ProjectRoot + "/";
                }

                return m_projectRootWithSplit;
            }
        }

        /// <summary>
        /// 打开指定文件夹(编辑器模式下)
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = path.Replace("/", "\\");
            if (!Directory.Exists(path))
            {
                Debug.LogError("No Directory: " + path);
                return;
            }

            if (!path.StartsWith("file://"))
            {
                path = "file://" + path;
            }

            Application.OpenURL(path);
        }

        public static T GetScriptableObjectAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public static void CreateOrReplacePrefab(GameObject gameobject, string path,
            ReplacePrefabOptions options = ReplacePrefabOptions.ConnectToPrefab)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                PrefabUtility.ReplacePrefab(gameobject, prefab, options);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                PrefabUtility.CreatePrefab(path, gameobject, options);
            }
        }

        #region 打包相关方法实现

        public static void CopyAssetBundlesTo(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var source = ColaEditHelper.GetAssetBundleDirectory();
            if (!Directory.Exists(source))
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
            if (Directory.Exists(outputPath))
                FileUtil.DeleteFileOrDirectory(outputPath);
            FileUtil.CopyFileOrDirectory(source, outputPath);
        }

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }

        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
                    return "OSX";
#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return "OSX";
#endif
                default:
                    return null;
            }
        }

        public static string CreateAssetBundleDirectory()
        {
            // Choose the output path according to the build target.
            var outputPath = Utility.AssetBundles + "/" + GetPlatformName();
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        }

        public static string GetAssetBundleDirectory()
        {
            return Utility.AssetBundles + "/" + GetPlatformName();
        }

        private static Dictionary<string, string> GetVersions(AssetBundleManifest manifest)
        {
            var items = manifest.GetAllAssetBundles();
            return items.ToDictionary(item => item, item => manifest.GetAssetBundleHash(item).ToString());
        }

        private static Dictionary<string, ABFileInfo> LoadVersions(string versionsTxt)
        {
            if (!File.Exists(versionsTxt))
                return null;
            return FileHelper.ReadABVersionInfo(versionsTxt);
        }

        private static void SaveVersions(string versionsTxt, string path, Dictionary<string, string> versions)
        {
            if (File.Exists(versionsTxt))
                File.Delete(versionsTxt);
            var sb = new StringBuilder(64);
            foreach (var item in versions)
            {
                var fileSize = FileHelper.GetFileSizeKB(path + "/" + item.Key);
                sb.Append(string.Format("{0}:{1}:{2}:{3}\n", item.Key, item.Value, fileSize, fileSize));
            }

            FileHelper.WriteString(versionsTxt, sb.ToString());
        }

        public static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        public static void SetAssetBundleNameAndVariant(string assetPath, string bundleName, string variant)
        {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) return;
            importer.assetBundleName = bundleName;
            if (!string.IsNullOrEmpty(variant))
            {
                importer.assetBundleVariant = variant;
            }
        }

        /// <summary>
        /// 生成asset与abpath映射Manifest
        /// </summary>
        public static void BuildManifest()
        {
            var manifest = GetManifest();

            var assetPath = AssetDatabase.GetAssetPath(manifest);
            var bundleName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            bundleName += AppConst.ExtName;
            SetAssetBundleNameAndVariant(assetPath, bundleName, null);

            AssetDatabase.RemoveUnusedAssetBundleNames();
            var bundles = AssetDatabase.GetAllAssetBundleNames();

            List<string> dirs = new List<string>();
            List<AssetData> assets = new List<AssetData>();

            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundles[i].StartsWith(AppConst.LuaBundlePrefix))
                {
                    //Lua AssetBundle不需要生成映射Manifest
                    continue;
                }

                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[i]);
                foreach (var path in paths)
                {
                    var dir = TrimedAssetDirName(path);
                    var index = dirs.FindIndex((o) => o.Equals(dir));
                    if (index == -1)
                    {
                        index = dirs.Count;
                        dirs.Add(dir);
                    }

                    var asset = new AssetData();
                    asset.bundle = i;
                    asset.dir = index;
                    asset.name = Path.GetFileName(path);

                    assets.Add(asset);
                }
            }

            manifest.bundles = bundles;
            manifest.dirs = dirs.ToArray();
            manifest.assets = assets.ToArray();

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string TrimedAssetDirName(string assetDirName)
        {
            assetDirName = assetDirName.Replace("\\", "/");
            assetDirName = assetDirName.Replace(Constants.GameAssetBasePath, "");
            return Path.GetDirectoryName(assetDirName).Replace("\\", "/");
            ;
        }

        public static void BuildAssetBundles()
        {
            // Choose the output path according to the build target.
            var outputPath = CreateAssetBundleDirectory();

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

            var manifest =
                BuildPipeline.BuildAssetBundles(outputPath, options,
                    EditorUserBuildSettings.activeBuildTarget);
            var versionsTxt = outputPath + "/versions.txt";
            var versions = LoadVersions(versionsTxt);

            var buildVersions = GetVersions(manifest);

            var updates = new List<string>();

            foreach (var item in buildVersions)
            {
                ABFileInfo abInfo;
                var isNew = true;
                if (null != versions && versions.TryGetValue(item.Key, out abInfo))
                {
                    string hash = abInfo.md5;
                    if (hash.Equals(item.Value))
                        isNew = false;
                }

                if (isNew)
                    updates.Add(item.Key);
            }

            if (updates.Count > 0)
            {
                using (var s = new StreamWriter(File.Open(outputPath + "/updates.txt", FileMode.Create)))
                {
                    //s.WriteLine(DateTime.Now.ToFileTime() + ":");
                    foreach (var item in updates)
                        s.WriteLine(item);
                    s.Flush();
                    s.Close();
                }

                SaveVersions(versionsTxt, outputPath, buildVersions);
            }
            else
            {
                Debug.Log("nothing to update.");
            }

            // string[] ignoredFiles = {GetPlatformName(), "versions.txt", "updates.txt", "manifest"};
            //
            // var files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories);
            //
            // var deletes = (from t in files
            //     let file = t.Replace('\\', '/').Replace(outputPath.Replace('\\', '/') + '/', "")
            //     where !file.EndsWith(".manifest", StringComparison.Ordinal) &&
            //           !Array.Exists(ignoredFiles, s => s.Equals(file))
            //     where !buildVersions.ContainsKey(file)
            //     select t).ToList();
            //
            // foreach (var delete in deletes)
            // {
            //     if (!File.Exists(delete))
            //         continue;
            //     File.Delete(delete);
            //     File.Delete(delete + ".manifest");
            // }
            //
            // deletes.Clear();
        }

        public static AssetsManifest GetManifest()
        {
            return GetScriptableObjectAsset<AssetsManifest>(Utility.AssetsManifestAsset);
        }

        public static string GetServerURL()
        {
            string downloadURL;
            if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
            {
                downloadURL = overloadedDevelopmentServerURL;
            }
            else
            {
                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }

                downloadURL = "http://" + localIP + ":7888/";
            }

            return downloadURL;
        }

        #region 处理Lua代码

        public static void BuildLuaBundle(bool isMotherPkg = false, bool isHotUpdateBuild = false)
        {
            var md5Dic = new Dictionary<string, string>();
            var luaMd5FilePath = ColaEditHelper.TempCachePath + "/LuaMD5.txt";
            bool needDiff = isMotherPkg || isHotUpdateBuild;
            var basePath = isHotUpdateBuild ? LuaConst.luaUpdateTempDir : LuaConst.luaBaseTempDir;

            if (needDiff)
            {
                Debug.Log("=================Need Diff Lua================");
                if (File.Exists(luaMd5FilePath))
                {
                    using (var sm = new StreamReader(luaMd5FilePath, Encoding.UTF8))
                    {
                        var fileLines = sm.ReadToEnd().Split('\n');
                        foreach (var item in fileLines)
                        {
                            if (string.IsNullOrEmpty(item))
                            {
                                continue;
                            }

                            var lineContent = item.Split('|');
                            if (lineContent.Length == 2)
                            {
                                md5Dic[lineContent[0]] = lineContent[1];
                            }
                            else
                            {
                                Debug.LogError("LuaMD5.txt格式错误！内容为: " + lineContent);
                            }
                        }
                    }
                }
            }

            //合并Lua代码，并复制到临时目录中准备打包
            if (!isHotUpdateBuild)
            {
                FileHelper.RmDir(LuaConst.luaBaseTempDir);
            }

            FileHelper.EnsureParentDirExist(LuaConst.luaBaseTempDir);
            FileHelper.RmDir(LuaConst.luaUpdateTempDir);
            FileHelper.EnsureParentDirExist(LuaConst.luaUpdateTempDir);
            AssetDatabase.Refresh();

            string[] srcDirs = { LuaConst.toluaDirWithSpliter, LuaConst.luaDirWithSpliter };
            int diffCnt = 0;

            for (int i = 0; i < srcDirs.Length; i++)
            {
                if (AppConst.LuaByteMode)
                {
                    string sourceDir = srcDirs[i];
                    string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                    int len = sourceDir.Length;

                    if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
                    {
                        --len;
                    }

                    for (int j = 0; j < files.Length; j++)
                    {
                        string str = files[j].Remove(0, len);
                        string dest = LuaConst.luaBaseTempDir + str + ".bytes";
                        string dir = Path.GetDirectoryName(dest);
                        Directory.CreateDirectory(dir);
                        EncodeLuaFile(files[j], dest);
                    }
                }
                else
                {
                    string[] files = FileHelper.GetAllChildFiles(srcDirs[i], "lua");

                    foreach (var fileName in files)
                    {
                        if (needDiff)
                        {
                            string curMd5 = FileHelper.GetMD5Hash(fileName);
                            if (isHotUpdateBuild && md5Dic.ContainsKey(fileName) && curMd5 == md5Dic[fileName])
                            {
                                continue;
                            }

                            if (isMotherPkg)
                            {
                                md5Dic[fileName] = curMd5;
                            }

                            if (isHotUpdateBuild)
                            {
                                diffCnt++;
                            }
                        }

                        var reltaFileName = fileName.Replace(srcDirs[i], "");
                        var dirName = Path.GetDirectoryName(reltaFileName);
                        if (!string.IsNullOrEmpty(dirName))
                        {
                            dirName = dirName.Replace("\\", "/");
                            if (!dirName.EndsWith("/"))
                            {
                                dirName += "/";
                            }

                            dirName = dirName.Replace("/", ".");
                        }

                        var dest = basePath + dirName + Path.GetFileName(reltaFileName) + ".bytes";
                        File.Copy(fileName, dest, true);
                    }
                }
            }

            if (isMotherPkg)
            {
                var sb = new StringBuilder();
                foreach (var item in md5Dic)
                {
                    sb.AppendFormat("{0}|{1}", item.Key, item.Value).AppendLine();
                }

                FileHelper.EnsureParentDirExist(luaMd5FilePath);
                FileHelper.WriteString(luaMd5FilePath, sb.ToString());
            }

            if (isHotUpdateBuild)
            {
                Debug.LogFormat("Lua差异化分析完毕！共有{0}个差异化文件！", diffCnt);
            }

            AssetDatabase.Refresh();
            //标记ABName
            var LuaBundleName = isHotUpdateBuild ? AppConst.LuaUpdateBundle : AppConst.LuaBaseBundle;
            MarkAssetsToOneBundle(basePath, LuaBundleName);
            AssetDatabase.Refresh();
        }

        public static void BuildLuaFile(bool isMotherPkg = false, bool isHotUpdateBuild = false)
        {
            //合并Lua代码，并复制到StreamingAsset目录中准备打包
            FileHelper.RmDir(LuaConst.streamingAssetLua);
            FileHelper.EnsureParentDirExist(LuaConst.streamingAssetLua);

            string[] luaPaths = { LuaConst.toluaDirWithSpliter, LuaConst.luaDirWithSpliter };

            var paths = new List<string>();
            var files = new List<string>();

            for (int i = 0; i < luaPaths.Length; i++)
            {
                paths.Clear();
                files.Clear();
                string luaDataPath = luaPaths[i].ToLower();
                FileHelper.Recursive(luaDataPath, files, paths);
                foreach (string f in files)
                {
                    if (f.EndsWith(".meta")) continue;
                    string newfile = f.Replace(luaDataPath, "");
                    string newpath = LuaConst.streamingAssetLuaWithSpliter + newfile;
                    string path = Path.GetDirectoryName(newpath);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    if (File.Exists(newpath))
                    {
                        File.Delete(newpath);
                    }

                    if (AppConst.LuaByteMode)
                    {
                        EncodeLuaFile(f, newpath);
                    }
                    else
                    {
                        File.Copy(f, newpath, true);
                    }

                    EditorUtility.DisplayProgressBar("玩命处理中",
                        string.Format("正在处理第{0}Lua文件目录 {1}/{2}", i, i, luaPaths.Length), i * 1.0f / luaPaths.Length);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void EncodeLuaFile(string srcFile, string outFile)
        {
            //if (!srcFile.ToLower().EndsWith(".lua"))
            //{
            //    File.Copy(srcFile, outFile, true);
            //    return;
            //}
            //bool isWin = true;
            //string luaexe = string.Empty;
            //string args = string.Empty;
            //string exedir = string.Empty;
            //string currDir = Directory.GetCurrentDirectory();
            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{
            //    isWin = true;
            //    luaexe = "luajit.exe";
            //    args = "-b -g " + srcFile + " " + outFile;
            //    exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
            //}
            //else if (Application.platform == RuntimePlatform.OSXEditor)
            //{
            //    isWin = false;
            //    luaexe = "./luajit";
            //    args = "-b -g " + srcFile + " " + outFile;
            //    exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit_mac/";
            //}
            //Directory.SetCurrentDirectory(exedir);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = luaexe;
            //info.Arguments = args;
            //info.WindowStyle = ProcessWindowStyle.Hidden;
            //info.UseShellExecute = isWin;
            //info.ErrorDialog = true;
            //Util.Log(info.FileName + " " + info.Arguments);

            //Process pro = Process.Start(info);
            //pro.WaitForExit();
            //Directory.SetCurrentDirectory(currDir);
        }

        #endregion

        /// <summary>
        /// 清除所有的AB Name
        /// </summary>
        public static void ClearAllAssetBundleName()
        {
            string[] oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            var length = oldAssetBundleNames.Length;
            for (int i = 0; i < length; i++)
            {
                EditorUtility.DisplayProgressBar("清除AssetBundleName", "正在清除AssetBundleName", i * 1f / length);
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[i], true);
            }

            EditorUtility.ClearProgressBar();
        }

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            return assetBundleName.Replace(Constants.GameAssetBasePath, "");
        }

        /// <summary>
        /// 标记一个文件夹下所有文件为一个BundleName
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bundleName"></param>
        public static void MarkAssetsToOneBundle(string path, string bundleName)
        {
            if (Directory.Exists(path))
            {
                bundleName = bundleName.ToLower();
                var files = FileHelper.GetAllChildFiles(path);
                var length = files.Length;
                for (int i = 0; i < length; i++)
                {
                    var fileName = files[i];
                    if (fileName.EndsWith(".meta", StringComparison.Ordinal) ||
                        fileName.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("玩命处理中", string.Format("正在标记第{0}个文件... {1}/{2}", i, i, length),
                        i * 1.0f / length);
                    var assetPath = files[i].Replace(ProjectRootWithSplit, "");
                    SetAssetBundleNameAndVariant(assetPath, bundleName, null);
                }

                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 按文件夹进行标记AssetBundleName
        /// </summary>
        /// <param name="path"></param>
        public static void MarkAssetsWithDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError("MarkAssetsWithDir Error! Path: " + path + "is not exist!");
                return;
            }

            var files = FileHelper.GetAllChildFiles(path);
            var length = files.Length;
            for (var i = 0; i < files.Length; i++)
            {
                var fileName = files[i];
                if (fileName.EndsWith(".meta", StringComparison.Ordinal) ||
                    fileName.EndsWith(".cs", System.StringComparison.CurrentCulture))
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("玩命处理中", string.Format("正在标记第{0}个文件... {1}/{2}", i, i, length),
                    i * 1.0f / length);
                var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(fileName).Replace("\\", "/")) + "_g" +
                                      AppConst.ExtName;
                var assetPath = fileName.Replace(ProjectRootWithSplit, "");
                SetAssetBundleNameAndVariant(assetPath, assetBundleName.ToLower(), null);
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 按文件进行标记AssetBundleName
        /// </summary>
        /// <param name="path"></param>
        public static void MarkAssetsWithFile(string path)
        {
            if (File.Exists(path))
            {
                if (path.EndsWith(".meta", StringComparison.Ordinal) ||
                    path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                {
                    return;
                }

                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                if (dir == null)
                    return;
                if (name == null)
                    return;
                dir = dir.Replace("\\", "/") + "/";
                var assetBundleName = TrimedAssetBundleName(Path.Combine(dir, name)) + AppConst.ExtName;
                SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            else if (Directory.Exists(path))
            {
                var files = FileHelper.GetAllChildFiles(path);
                var length = files.Length;
                for (var i = 0; i < files.Length; i++)
                {
                    var fileName = files[i];
                    if (fileName.EndsWith(".meta", StringComparison.Ordinal) ||
                        fileName.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("玩命处理中", string.Format("正在标记第{0}个文件... {1}/{2}", i, i, length),
                        i * 1.0f / length);

                    var dir = Path.GetDirectoryName(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    if (dir == null)
                        return;
                    if (name == null)
                        return;
                    dir = dir.Replace("\\", "/") + "/";
                    var assetBundleName = TrimedAssetBundleName(Path.Combine(dir, name)) + AppConst.ExtName;
                    var assetPath = fileName.Replace(ProjectRootWithSplit, "");
                    SetAssetBundleNameAndVariant(assetPath, assetBundleName.ToLower(), null);
                }
            }
            else
            {
                Debug.LogError("MarkAssetsWithFile Error! Path: " + path + "is not exist!");
                return;
            }

            EditorUtility.ClearProgressBar();
        }

        #endregion
    }
}