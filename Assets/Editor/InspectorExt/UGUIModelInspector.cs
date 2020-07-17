//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.UI.Extensions;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// UGUIModel组件的Inspector编辑器拓展
    /// </summary>
    [CustomEditor(typeof(UGUIModel), true)]
    public class UGUIModelInspector : InspectorBase
    {
        private UGUIModel model;

        protected override void OnEnable()
        {
            ShowCustomProperties = true;
            model = target as UGUIModel;
            if (model.gameObject.activeSelf)
            {
                model.InitInEditor(ColaGUIEditor.UICamera);
            }
        }

        protected override void DrawCustomGUI()
        {
            if (model)
            {
                var cameraYaw = serializedObject.FindProperty("cameraYaw");
                cameraYaw.floatValue = EditorGUILayout.Slider("相机Y轴旋转参数:", cameraYaw.floatValue, 0, 180);
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("导入设置"))
            {
                this.OnEnable();
                if (model)
                {
                    model.ImportModelInEditor();
                }
            }

            if (GUILayout.Button("导出设置"))
            {
                if (model)
                {
                    model.SaveSetting();
                }
            }

            if (GUILayout.Button("恢复默认设置"))
            {
                if (model)
                {
                    model.DefaultSetting();
                }
            }

            UpdateModel();
        }

        /// <summary>
        /// 更新编辑器中的模型信息
        /// </summary>
        private void UpdateModel()
        {
            if (model && model.gameObject && model.gameObject.activeSelf)
            {
                model.UpdateInEditor(ColaGUIEditor.GetScreenPixelDimensions());
            }
        }

        private void DrawProgressBar(string label, float value)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
    }
}

