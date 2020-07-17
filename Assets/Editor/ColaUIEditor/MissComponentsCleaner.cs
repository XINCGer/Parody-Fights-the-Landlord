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
    /// 预制体组件丢失清理助手类
    /// </summary>
    public class MissComponentsCleaner
    {
        [MenuItem("ColaFramework/Cleaner/清除丢失组件")]
        public static void ClearMissComonents()
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep);
            for (int i = 0; i < transforms.Length; i++)
            {
                EditorUtility.DisplayProgressBar("清理组件中...", "清理重复组件:" + transforms[i].name, i / (float)transforms.Length);
                ClearMissComponents(transforms[i].gameObject);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void ClearMissComponents(GameObject obj)
        {
            if (null == obj)
            {
                return;
            }
            var components = obj.GetComponents<Component>();
            SerializedObject serializedObject = new SerializedObject(obj);
            SerializedProperty props = serializedObject.FindProperty("m_Component");
            int offset = 0;
            for (int j = 0; j < components.Length; j++)
            {
                if (null == components[j])
                {
                    props.DeleteArrayElementAtIndex(j - offset);
                    offset++;
                    Debug.LogWarning(string.Format("移除丢失组件:{0}", obj.name));
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("ColaFramework/Cleaner/ResetAll预制")]
        public static void PrefabApply()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Reset组件...", "Apply组件:" + objs[i].name, i / (float)objs.Length);
                SavePrefabObj(objs[i]);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void SavePrefabObj(GameObject go)
        {
            Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (null == prefabParent)
            {
                Debug.LogError("Can not find prefab from obj:" + go.name);
                return;
            }
            PrefabUtility.ReplacePrefab(go, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
        }
    }
}