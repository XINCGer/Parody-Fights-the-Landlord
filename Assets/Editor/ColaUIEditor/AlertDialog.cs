//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 编辑器中的弹出提示框
    /// </summary>
    public class AlertDialog : EditorWindow
    {
        private static AlertDialog window;
        private string alertText;
        private Color defaultColor;
        private Color textColor;
        private Dictionary<AlertLevel, Color> textColorDic = new Dictionary<AlertLevel, Color>()
        {
            {AlertLevel.Info, Color.white },
            {AlertLevel.Debug, Color.green },
            {AlertLevel.Warn, Color.yellow },
            {AlertLevel.Error, Color.red },
        };

        public static void PopUp(string alertText, AlertLevel alertLevel = AlertLevel.Info)
        {
            window = EditorWindow.GetWindow(typeof(AlertDialog), true, "提示框") as AlertDialog;
            window.position = new Rect(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(500, 100));
            window.SetText(alertText,alertLevel);
            window.Show();
            window.Focus();
        }

        private void SetText(string alertText,AlertLevel alertLevel)
        {
            this.alertText = alertText;
            defaultColor = GUI.color;
            textColor = textColorDic[alertLevel];
        }

        private void OnGUI()
        {
            DrawEditorGUI();
        }

        private void DrawEditorGUI()
        {
            GUI.color = textColor;
            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(alertText);
            GUILayout.EndHorizontal();
            GUI.color = defaultColor;
        }

        private void OnDestroy()
        {
            GUI.color = defaultColor;
        }

    }

    /// <summary>
    /// 提示的级别
    /// </summary>
    public enum AlertLevel : byte
    {
        /// <summary>
        /// 普通信息，白色
        /// </summary>
        Info = 1,
        /// <summary>
        /// 调试信息，绿色
        /// </summary>
        Debug = 2,
        /// <summary>
        /// 警告信息，黄色
        /// </summary>
        Warn = 3,
        /// <summary>
        /// 错误信息，红色
        /// </summary>
        Error = 4,
    }

}

