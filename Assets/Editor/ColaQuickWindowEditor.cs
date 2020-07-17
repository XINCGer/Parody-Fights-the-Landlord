//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using ColaFramework;
using System.Text;
using ColaFramework.Foundation;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;

namespace ColaFramework.ToolKit
{
    public class ColaQuickWindowEditor : EditorWindow
    {
        /// <summary>
        /// Lua业务逻辑代码的路径
        /// </summary>
        private const string LuaLogicPath = "Assets/Lua";

        [MenuItem("ColaFramework/Open Quick Window %Q")]
        static void Popup()
        {
            ColaQuickWindowEditor window = EditorWindow.GetWindow<ColaQuickWindowEditor>();
            window.titleContent = new GUIContent("快捷工具窗");
            window.position = new Rect(400, 100, 640, 480);
            window.Show();
        }

        public void OnGUI()
        {
            DrawColaFrameworkUI();
            GUILayout.Space(20);
            DrawMiscUI();
            GUILayout.Space(20);
            DrawAssetUI();
        }


        public void DrawColaFrameworkUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField("== UI相关辅助 ==");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建NewUIView", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaGUIEditor.CreateColaUIView();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建C#版UIView脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                CreateScriptsEditor.CreateCSharpUIView();
            }
            if (GUILayout.Button("创建C#版Module脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                CreateScriptsEditor.CreateCSharpModule();
            }
            if (GUILayout.Button("创建C#版Templates(UIView和Module)", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                CreateScriptsEditor.CreateCSharpModule();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawMiscUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField("== 快捷功能 ==");
            GUILayout.EndHorizontal();
            if (GUILayout.Button("GC"))
            {
                CommonHelper.ClearMemory();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开AssetPath目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.OpenDirectory(AppConst.AssetPath);
            }
            if (GUILayout.Button("打开GameLog文件目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.OpenDirectory(Path.Combine(AppConst.AssetPath, "logs"));
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAssetUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField("== 快捷功能 ==");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Lua Bundle", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.BuildLuaBundle();
            }
            if (GUILayout.Button("Build Lua File", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.BuildLuaFile();
            }
            if (GUILayout.Button("Mark Lua Bundle", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.MarkAssetsToOneBundle(LuaConst.luaBaseTempDir, AppConst.LuaBaseBundle);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Mark Sprite Bundle", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                ColaEditHelper.MarkAssetsWithDir("Assets/GameAssets/Arts/UI/Atlas/");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出版本Json", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                var assetPath = "Assets/Editor/Settings/AppVersion.asset";
                var asset = AssetDatabase.LoadAssetAtPath<AppVersion>(assetPath);
                if (null != asset)
                {
                    var jsonStr = JsonMapper.ToJson(asset);
                    FileHelper.DeleteFile("Assets/Resources/app_version.json");
                    FileHelper.WriteString("Assets/Resources/app_version.json", jsonStr);
                    AssetDatabase.Refresh();
                }
            }
            if (GUILayout.Button("导入Json文件", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                var jsonPath = "Assets/Editor/Settings/AppVersion.json";
                using (var sr = new StreamReader(jsonPath))
                {
                    var jsonStr = sr.ReadToEnd();
                    var asset = JsonMapper.ToObject<AppVersion>(jsonStr);
                    Debug.Log(asset.UpdateContent);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Zip Lua", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                var result = ZipHelper.Zip("Assets/Lua", Path.Combine(Application.dataPath, "../output/luaout.zip"));
                Debug.Log("Zip Lua结果:" + result);
            }
            if (GUILayout.Button("UnZip Lua", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                var filePath = Path.Combine("Assets", "../output/luaout.zip");
                if (File.Exists(filePath))
                {
                    var result = ZipHelper.UnZip(filePath, Path.Combine("Assets", "../output"));
                    Debug.Log("UnZip Lua结果:" + result);
                }
                else
                {
                    Debug.LogError("解压错误！要解压的文件不存在！路径:" + filePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("筛选出MD5码变化的lua文件", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
            {
                var md5Dic = new Dictionary<string, string>();
                var luaMd5FilePath = ColaEditHelper.TempCachePath + "/LuaMD5.txt";
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

                var luaFiles = new List<string>(Directory.GetFiles(LuaLogicPath, "*.lua", SearchOption.AllDirectories));
                var fLength = (float)luaFiles.Count;

                int diffCnt = 0;
                for (int i = 0; i < luaFiles.Count; i++)
                {
                    var fileName = luaFiles[i];
                    string curMd5 = FileHelper.GetMD5Hash(fileName);
                    if (md5Dic.ContainsKey(fileName) && curMd5 == md5Dic[fileName])
                    {
                        continue;
                    }
                    diffCnt++;
                    string destPath = Regex.Replace(fileName, "^Assets", "output");
                    FileHelper.EnsureParentDirExist(destPath);
                    File.Copy(fileName, destPath, true);
                    md5Dic[fileName] = curMd5;
                    EditorUtility.DisplayProgressBar("正在分析Lua差异化..", fileName, i / fLength);

                }

                var sb = new StringBuilder();
                foreach (var item in md5Dic)
                {
                    sb.AppendFormat("{0}|{1}", item.Key, item.Value).AppendLine();
                }
                FileHelper.WriteString(luaMd5FilePath, sb.ToString());
                EditorUtility.ClearProgressBar();

                Debug.LogFormat("Lua差异化分析完毕！共有{0}个差异化文件！", diffCnt);
            }
            GUILayout.EndHorizontal();
        }
    }
}
