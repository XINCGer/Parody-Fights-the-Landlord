//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace ColaFramework.ToolKit
{
    [InitializeOnLoad]
    public class SoarDHierarchy
    {

        static GameObject[] SelectionObjs;
        static Dictionary<GameObject, Rect> GO2Rect = new Dictionary<GameObject, Rect>();

        static SoarDHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            Selection.selectionChanged += OnSelectionChanged;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            CustomItem(selectionRect, obj);
        }

        static void CustomItem(Rect selectionRect, Object obj)
        {
            if (obj is GameObject)
            {
                GameObject go = obj as GameObject;

                if (!GO2Rect.ContainsKey(go))
                {
                    GO2Rect.Add(go, selectionRect);
                }

                ShowOrHideGameObject(go, selectionRect);
                CalculateChildNode(go, selectionRect);
            }
        }

        static void ShowOrHideGameObject(GameObject go, Rect selectionRect)
        {
            Rect r = new Rect(selectionRect);
            r.x += r.width - 30;

            bool isActive = GUI.Toggle(r, go.activeSelf, "");

            if (isActive != go.activeSelf)
            {
                if (SelectionObjs != null && SelectionObjs.Length > 1)
                {
                    for (int i = 0; i < SelectionObjs.Length; i++)
                    {
                        if (SelectionObjs[i] != go)
                        {
                            SelectionObjs[i].SetActive(isActive);
                        }
                    }
                }
            }

            go.SetActive(isActive);
        }

        static void CalculateChildNode(GameObject go, Rect selectionRect)
        {

            Transform trans = go.transform;

            Rect r = new Rect(selectionRect);
            r.x -= 35;
            int childCount = 0;
            Transform[] childs = trans.GetComponentsInChildren<Transform>(true);

            childCount = childs.Length - 1;
            TextAnchor ta = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUI.Label(r, childCount == 0 ? "" : childCount.ToString());
            GUI.skin.label.alignment = ta;
        }

        static void OnSelectionChanged()
        {
            SelectionObjs = Selection.gameObjects;
        }
    }
}