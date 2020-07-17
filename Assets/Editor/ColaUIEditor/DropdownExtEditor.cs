//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI.Extensions;

namespace ColaFramework.ToolKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DropdownExtension))]
    public class DropdownExtEditor : DropdownEditor
    {
        SerializedProperty openMark;
        SerializedProperty closeMark;
        SerializedProperty useContentSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            // Setup the SerializedProperties.
            openMark = serializedObject.FindProperty("openMark");
            closeMark = serializedObject.FindProperty("closeMark");
            useContentSize = serializedObject.FindProperty("useContentSize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(openMark, new GUIContent("openMark"));
            EditorGUILayout.PropertyField(closeMark, new GUIContent("closeMark"));
            EditorGUILayout.PropertyField(useContentSize, new GUIContent("useContentSize"));

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}