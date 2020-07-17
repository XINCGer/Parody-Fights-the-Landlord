//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;

#if UNITY_EDITOR

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 用于创建代码模版的编辑器类(UI\Model等类)
    /// </summary>
    public static class CreateScriptsEditor
    {
        #region 共有变量
        private static readonly string CSharpTemplateUIViewPath = "Assets/Editor/CreateScriptEditor/Templates/CSharp/UIViewTemplate.txt";
        private static readonly string CSharpTemplateModulePath = "Assets/Editor/CreateScriptEditor/Templates/CSharp/ModuleTemplate.txt";
        private static readonly string LuaTemplateModulePath = "Assets/Editor/CreateScriptEditor/Templates/Lua/ModuleTemplate.txt";
        private static readonly string LuaTemplateControllerPath = "Assets/Editor/CreateScriptEditor/Templates/Lua/ControllerTemplate.txt";
        private static readonly string LuaTemplateViewPath = "Assets/Editor/CreateScriptEditor/Templates/Lua/UIViewTemplate.txt";
        #endregion

        #region 共用方法
        /// <summary>
        /// 获取在编辑器中选择的路径
        /// </summary>
        /// <returns></returns>
        static string GetSelectedPath()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
        #endregion

        #region 创建C#模版
        [MenuItem("Assets/Create/C#/UIView", false, 90)]
        public static void CreateCSharpUIView()
        {
            string basePath = GetSelectedPath();
            string templateFullPath = CSharpTemplateUIViewPath;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CreateCSharpScriptEndAction>(),
                basePath + "/NewUIView.cs",
                null,
                templateFullPath);
        }

        [MenuItem("Assets/Create/C#/Module", false, 91)]
        public static void CreateCSharpModule()
        {
            string basePath = GetSelectedPath();
            string templateFullPath = CSharpTemplateModulePath;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CreateCSharpScriptEndAction>(),
                basePath + "/NewModule.cs",
                null,
                templateFullPath);
        }

        [MenuItem("Assets/Create/C#/Templates(UIView和Module)", false, 92)]
        public static void CreateTemplates()
        {
            string basePath = GetSelectedPath();
            //获取最后一级文件夹名，即选中的文件夹的名称
            string dirName = basePath.Substring(basePath.LastIndexOf(@"/") + 1);
            //创建对应的View和Module子路径
            string uiviewPath = Path.Combine(GetSelectedPath(), "View/");
            string modulePath = Path.Combine(GetSelectedPath(), "Module/");
            string dataPath = Path.Combine(GetSelectedPath(), "Data/");
            CommonHelper.CheckLocalFileExist(uiviewPath);
            CommonHelper.CheckLocalFileExist(modulePath);
            CommonHelper.CheckLocalFileExist(dataPath);

            //拷贝模板文件并创建新的文件
            string uiviewFileName = uiviewPath + dirName + "_UIView.cs";
            string moduleFileName = modulePath + dirName + "_Module.cs";
            CreateCSharpScriptEndAction.CreateScriptAssetFromTemplate(uiviewFileName, CSharpTemplateUIViewPath);
            CreateCSharpScriptEndAction.CreateScriptAssetFromTemplate(moduleFileName, CSharpTemplateModulePath);

            //刷新资源
            AssetDatabase.Refresh();
        }

        #endregion

        #region 创建Lua模版
        [MenuItem("Assets/Create/Lua/UIView", false, 93)]
        public static void CreateLuaUIView()
        {
            string basePath = GetSelectedPath();
            string templateFullPath = LuaTemplateViewPath;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CreateLuaScriptEndAction>(),
                basePath + "/NewUIView.lua",
                null,
                templateFullPath);
        }

        [MenuItem("Assets/Create/Lua/Module", false, 94)]
        public static void CreateLuaModule()
        {
            string basePath = GetSelectedPath();
            string templateFullPath = LuaTemplateModulePath;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CreateLuaScriptEndAction>(),
                basePath + "/New_Module.lua",
                null,
                templateFullPath);
        }

        [MenuItem("Assets/Create/Lua/Controller", false, 95)]
        public static void CreateLuaController()
        {
            string basePath = GetSelectedPath();
            string templateFullPath = LuaTemplateControllerPath;
            Debug.Log(basePath);
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CreateLuaScriptEndAction>(),
                basePath + "/New_Controller.lua",
                null,
                templateFullPath);
        }

        [MenuItem("Assets/Create/Lua/Templates(Module和Controller)", false, 95)]
        public static void CreateLuaTemplate()
        {
            string basePath = GetSelectedPath();
            //获取最后一级文件夹名，即选中的文件夹的名称
            string dirName = basePath.Substring(basePath.LastIndexOf(@"/") + 1);

            //拷贝模板文件并创建新的文件
            string moduleFilePath = basePath + "/" + dirName + "_Module.lua";
            string controllerFilePath = basePath + "/" + dirName + "_Controller.lua";
            CreateCSharpScriptEndAction.CreateScriptAssetFromTemplate(moduleFilePath, LuaTemplateModulePath);
            CreateCSharpScriptEndAction.CreateScriptAssetFromTemplate(controllerFilePath, LuaTemplateControllerPath);

            //刷新资源
            AssetDatabase.Refresh();
        }

        #endregion
    }

#endif
}