//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using ColaFramework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using System;
using UnityEngine.UI;
using System.Text;
using ColaFramework.Foundation;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// ColaFramework框架中编辑器模式下的UI创建、获取UIRoot等
    /// </summary>
    public class ColaGUIEditor
    {
        private static bool editorgetGameViewSizeError = false;
        public static bool editorgameViewReflectionError = false;

        private static Dictionary<string, int> uiExportElementDic = new Dictionary<string, int>();
        private static List<Type> ExportComponentTypes = new List<Type>() { typeof(IControl),typeof(Button),typeof(InputField),typeof(Dropdown),typeof(Toggle),typeof(Slider),
        typeof(ScrollRect),typeof(Scrollbar)};
        private static List<Type> ExportPropertyTypes = new List<Type>() { typeof(IComponent), typeof(Image), typeof(RawImage), typeof(Text), typeof(RectTransform), typeof(Transform) };
        private static UIComponentCollection UICollection;
        private static int UIComponentIndex = -1;

        private class ExportInfo
        {
            public string name;
            public Dictionary<string, int> uiExportElementDic = new Dictionary<string, int>();
            public UIComponentCollection uicollection;
            public int index = -1;
        }

        private static List<ExportInfo> nestExportInfoList = new List<ExportInfo>(2);

        public static Camera UICamera
        {
            get { return GetOrCreateUICamera(); }
        }

        /// <summary>
        /// 快速创建UI模版
        /// </summary>
        [MenuItem("GameObject/UI/ColaUI/UIView", false, 1)]
        public static void CreateColaUIView()
        {
            GameObject uguiRoot = GetOrCreateUGUIRoot();

            //创建新的UI Prefab
            GameObject view = new GameObject("NewUIView", typeof(RectTransform));
            view.tag = Constants.UIViewTag;
            view.layer = LayerMask.NameToLayer("UI");
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(uguiRoot.transform, view.name);
            view.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(view, "Create" + view.name);
            Undo.SetTransformParent(view.transform, uguiRoot.transform, "Parent" + view.name);
            GameObjectUtility.SetParentAndAlign(view, uguiRoot);

            //设置RectTransform属性
            RectTransform rect = view.GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);

            //设置新建的UIView被选中
            Selection.activeGameObject = view;
        }

        [MenuItem("GameObject/UI/ColaUI/ExportUIView", false, 2)]
        public static void ExportUIView()
        {
            var uiObj = Selection.activeGameObject;
            if (null == uiObj) return;
            var UIViewName = uiObj.name;
            if (!UIViewName.StartsWith("UI", StringComparison.CurrentCultureIgnoreCase))
            {
                UIViewName = "UI" + UIViewName;
            }
            UIViewName = UIViewName.Substring(0, 2).ToUpper() + UIViewName.Substring(2);  //ToUpperFirst

            uiObj.name = UIViewName;
            uiExportElementDic.Clear();
            nestExportInfoList.Clear();
            UICollection = null;
            UIComponentIndex = -1;

            ProcessUIPrefab(uiObj);
            GenUIViewCode(UIViewName);
            var prefabPath = Constants.UIExportPrefabPath + UIViewName + ".prefab";
            ColaEditHelper.CreateOrReplacePrefab(uiObj, prefabPath);
            AssetDatabase.Refresh();
        }

        private static void ProcessUIPrefab(GameObject gameObject)
        {
            if (null == gameObject) return;
            if (gameObject.CompareTag(Constants.UIViewTag))
            {
                UICollection = gameObject.AddSingleComponent<UIComponentCollection>();
                UICollection.Clear();
            }
            foreach (Transform transform in gameObject.transform)
            {
                if (transform.CompareTag(Constants.UIIgnoreTag))
                {
                    continue;
                }

                //处理UITableview

                if (transform.GetComponent<UITableViewCell>() != null)
                {
                    var info = new ExportInfo();
                    nestExportInfoList.Add(info);
                    var uiTableview = transform.gameObject.GetComponentInParent<UITableView>();
                    info.name = "m_" + uiTableview.transform.name;
                    var collection = transform.gameObject.AddSingleComponent<UIComponentCollection>();
                    collection.Clear();
                    info.uicollection = collection;
                    ProceseUITableview(transform.gameObject, info);
                }
                else
                {
                    ProcessUIPrefab(transform.gameObject);
                }

                bool isHandled = false;
                foreach (var type in ExportComponentTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp)
                    {
                        UIComponentIndex++;
                        UICollection.Add(UIComp);
                        var componentName = "m_" + transform.name;
                        uiExportElementDic[componentName] = UIComponentIndex;
                        isHandled = true;
                        break;
                    }
                }
                if (isHandled) continue;
                foreach (var type in ExportPropertyTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp && transform.CompareTag(Constants.UIPropertyTag))
                    {
                        UIComponentIndex++;
                        UICollection.Add(UIComp);
                        var componentName = "m_" + transform.name;
                        uiExportElementDic[componentName] = UIComponentIndex;
                        isHandled = true;
                        break;
                    }
                }
            }
        }

        private static void ProceseUITableview(GameObject root, ExportInfo exportInfo)
        {
            if (null == root || null == exportInfo)
            {
                return;
            }

            foreach (Transform transform in root.transform)
            {
                if (transform.CompareTag(Constants.UIIgnoreTag))
                {
                    continue;
                }
                ProceseUITableview(transform.gameObject, exportInfo);

                bool isHandled = false;
                foreach (var type in ExportComponentTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp)
                    {
                        exportInfo.index++;
                        exportInfo.uicollection.Add(UIComp);
                        var componentName = "m_" + transform.name;
                        exportInfo.uiExportElementDic[componentName] = exportInfo.index;
                        isHandled = true;
                        break;
                    }
                }
                if (isHandled) continue;
                foreach (var type in ExportPropertyTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp && transform.CompareTag(Constants.UIPropertyTag))
                    {
                        exportInfo.index++;
                        exportInfo.uicollection.Add(UIComp);
                        var componentName = "m_" + transform.name;
                        exportInfo.uiExportElementDic[componentName] = exportInfo.index;
                        isHandled = true;
                        break;
                    }
                }
            }
        }

        private static void GenUIViewCode(string UIViewName)
        {
            var codePath = Constants.UIExportLuaViewPath + UIViewName + "_BindView" + ".lua";

            StringBuilder sb = new StringBuilder(16);
            sb.Append("--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]\n\n");
            sb.Append("local public = {}\n");
            sb.Append("local cachedViews = nil\n\n");
            sb.Append("public.viewPath = \"" + Constants.UIExportPrefabReltaPath + UIViewName + ".prefab\"\n\n");

            //BindView
            sb.Append("function public.BindView(uiView, Panel)\n");
            sb.Append("\tcachedViews = {}\n");
            sb.Append("\tif nil ~= Panel then\n");
            sb.Append("\t\tlocal collection = Panel:GetComponent(\"UIComponentCollection\")\n");
            sb.Append("\t\tif nil ~= collection then\n");

            if (uiExportElementDic.Count > 0)
            {
                foreach (var item in uiExportElementDic)
                {
                    sb.Append("\t\t\tuiView.").Append(item.Key).Append(" = collection:Get(").Append(item.Value).Append(")\n");
                }
            }
            else
            {
                sb.Append("\t\t\t--pass\n");
            }

            sb.Append("\t\telse\n\t\t\terror(\"BindView Error! UIComponentCollection is nil!\")\n\t\tend\n");
            sb.Append("\telse\n\t\terror(\"BindView Error! Panel is nil!\")\n\tend\nend\n\n");

            //UnBindView
            sb.Append("function public.UnBindView(uiView)\n");
            sb.Append("\tcachedViews = nil\n");
            if (uiExportElementDic.Count > 0)
            {
                foreach (var item in uiExportElementDic)
                {
                    sb.Append("\tuiView.").Append(item.Key).Append(" = nil\n");
                }
            }
            else
            {
                sb.Append("\t\t\t--pass\n");
            }
            sb.Append("end\n\n");

            //UITableview
            if (nestExportInfoList.Count > 0)
            {
                sb.Append("function public.GetCellView(uiView,tableview, cell)\n");
                sb.Append("\tlocal cellView = cachedViews[cell]\n");
                sb.Append("\tif nil == cellView then\n\t\tcellView = {}\n");
                foreach (var item in nestExportInfoList)
                {
                    sb.Append("\t\tif tableview == ").Append("uiView.").Append(item.name).Append(" then\n");
                    sb.Append("\t\t\tlocal collection = cell:GetComponent(\"UIComponentCollection\")\n");
                    foreach (var element in item.uiExportElementDic)
                    {
                        sb.Append("\t\t\tcellView.").Append(element.Key).Append(" = collection:Get(").Append(element.Value).Append(")\n");
                    }
                    sb.Append("\t\tend\n\t\tcachedViews[cell] = cellView\n");
                }
                sb.Append("\tend\n\treturn cellView\nend\n\n");
            }

            sb.Append("return public");

            FileHelper.WriteString(codePath, sb.ToString());
        }

        /// <summary>
        /// 获取或者创建UGUIRoot（编辑器状态下）
        /// </summary>
        /// <returns></returns>
        public static GameObject GetOrCreateUGUIRoot()
        {
            GameObject uguiRootObj = GameObject.FindWithTag("UIRoot");
            if (uguiRootObj != null)
            {
                Canvas canvas = uguiRootObj.GetComponentInChildren<Canvas>();
                if (null != canvas && canvas.gameObject.activeInHierarchy)
                {
                    return canvas.gameObject;
                }
            }

            //如果以上步骤都没有找到，那就从Resource里面加载并实例化一个
            var uguiRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameAssets/Arts/UI/Prefabs/UGUIRoot.prefab");
            GameObject uguiRoot = CommonHelper.InstantiateGoByPrefab(uguiRootPrefab, null);
            GameObject canvasRoot = uguiRoot.GetComponentInChildren<Canvas>().gameObject;
            return canvasRoot;
        }

        private static Camera GetOrCreateUICamera()
        {
            var uiRoot = GetOrCreateUGUIRoot();
            return uiRoot.GetComponentByPath<Camera>("UICamera");
        }

        // 获得分辨率，当选择 Free Aspect 直接返回相机的像素宽和高
        public static Vector2 GetScreenPixelDimensions()
        {
            Vector2 dimensions = new Vector2(UICamera.pixelWidth, UICamera.pixelHeight);

#if UNITY_EDITOR
            // 获取编辑器 GameView 的分辨率
            float gameViewPixelWidth = 0, gameViewPixelHeight = 0;
            float gameViewAspect = 0;

            if (EditorGetGameViewSize(out gameViewPixelWidth, out gameViewPixelHeight, out gameViewAspect))
            {
                if (gameViewPixelWidth != 0 && gameViewPixelHeight != 0)
                {
                    dimensions.x = gameViewPixelWidth;
                    dimensions.y = gameViewPixelHeight;
                }
            }
#endif
            return dimensions;
        }

        // 尝试获取 GameView 的分辨率
        // 当正确获取到 GameView 的分辨率时，返回 true
        public static bool EditorGetGameViewSize(out float width, out float height, out float aspect)
        {
            try
            {
                editorgameViewReflectionError = false;

                System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Reflection.MethodInfo GetMainGameView = gameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                object mainGameViewInst = GetMainGameView.Invoke(null, null);
                if (mainGameViewInst == null)
                {
                    width = height = aspect = 0;
                    return false;
                }
                System.Reflection.FieldInfo s_viewModeResolutions = gameViewType.GetField("s_viewModeResolutions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (s_viewModeResolutions == null)
                {
                    System.Reflection.PropertyInfo currentGameViewSize = gameViewType.GetProperty("currentGameViewSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    object gameViewSize = currentGameViewSize.GetValue(mainGameViewInst, null);
                    System.Type gameViewSizeType = gameViewSize.GetType();
                    int gvWidth = (int)gameViewSizeType.GetProperty("width").GetValue(gameViewSize, null);
                    int gvHeight = (int)gameViewSizeType.GetProperty("height").GetValue(gameViewSize, null);
                    int gvSizeType = (int)gameViewSizeType.GetProperty("sizeType").GetValue(gameViewSize, null);
                    if (gvWidth == 0 || gvHeight == 0)
                    {
                        width = height = aspect = 0;
                        return false;
                    }
                    else if (gvSizeType == 0)
                    {
                        width = height = 0;
                        aspect = (float)gvWidth / (float)gvHeight;
                        return true;
                    }
                    else
                    {
                        width = gvWidth; height = gvHeight;
                        aspect = (float)gvWidth / (float)gvHeight;
                        return true;
                    }
                }
                else
                {
                    Vector2[] viewModeResolutions = (Vector2[])s_viewModeResolutions.GetValue(null);
                    float[] viewModeAspects = (float[])gameViewType.GetField("s_viewModeAspects", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                    string[] viewModeStrings = (string[])gameViewType.GetField("s_viewModeAspectStrings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                    if (mainGameViewInst != null
                        && viewModeStrings != null
                        && viewModeResolutions != null && viewModeAspects != null)
                    {
                        int aspectRatio = (int)gameViewType.GetField("m_AspectRatio", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(mainGameViewInst);
                        string thisViewModeString = viewModeStrings[aspectRatio];
                        if (thisViewModeString.Contains("Standalone"))
                        {
                            width = UnityEditor.PlayerSettings.defaultScreenWidth; height = UnityEditor.PlayerSettings.defaultScreenHeight;
                            aspect = width / height;
                        }
                        else if (thisViewModeString.Contains("Web"))
                        {
                            width = UnityEditor.PlayerSettings.defaultWebScreenWidth; height = UnityEditor.PlayerSettings.defaultWebScreenHeight;
                            aspect = width / height;
                        }
                        else
                        {
                            width = viewModeResolutions[aspectRatio].x; height = viewModeResolutions[aspectRatio].y;
                            aspect = viewModeAspects[aspectRatio];
                            // this is an error state
                            if (width == 0 && height == 0 && aspect == 0)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                if (editorgetGameViewSizeError == false)
                {
                    Debug.LogError("GameCamera.GetGameViewSize - has a Unity update broken this?\nThis is not a fatal error !\n" + e.ToString());
                    editorgetGameViewSizeError = true;
                }
                editorgameViewReflectionError = true;
            }
            width = height = aspect = 0;
            return false;
        }
    }
}
