//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    static internal class TableViewMenu
    {
        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private static Vector2 s_ThickGUIElementSize = new Vector2(kWidth, kThickHeight);
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";

        [MenuItem("GameObject/UI/Horizontal TableView", false)]
        static void AddHorizontalTableView(MenuCommand menuCommand)
        {
            GameObject horizontalTableViewRoot = CreateUIElementRoot("horizontal_tableView", menuCommand, s_ThickGUIElementSize);

            GameObject childTableRect = CreateUIObject("tablerect", horizontalTableViewRoot);

            GameObject childContent = CreateUIObject("content", childTableRect);

            // Set RectTransform to stretch
            RectTransform rectTransformScrollSnapRoot = horizontalTableViewRoot.GetComponent<RectTransform>();
            rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.pivot = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
            rectTransformScrollSnapRoot.sizeDelta = new Vector2(600f, 300f);

            Image image = horizontalTableViewRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.type = Image.Type.Sliced;
            image.color = new Color(1f, 1f, 1f, 0.392f);

            horizontalTableViewRoot.AddComponent<RectMask2D>();

            ScrollRect sr = horizontalTableViewRoot.AddComponent<ScrollRect>();
            sr.vertical = false;
            sr.horizontal = true;

            RectTransform rectTransformRect = childTableRect.GetComponent<RectTransform>();
            rectTransformRect.anchorMin = Vector2.zero;
            rectTransformRect.anchorMax = new Vector2(1f, 1f);
            rectTransformRect.pivot = new Vector2(0.5f, 0.5f);
            rectTransformRect.sizeDelta = Vector2.zero;

            sr.viewport = rectTransformRect;

            //Setup Content container
            RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
            rectTransformContent.anchorMin = new Vector2(0, 0.5f);
            rectTransformContent.anchorMax = new Vector2(0f, 0.5f);
            rectTransformContent.pivot = new Vector2(0, 0.5f);
            rectTransformContent.sizeDelta = new Vector2(600f, 300f);

            GridLayoutGroup group = childContent.AddComponent<GridLayoutGroup>();
            group.cellSize = new Vector2(100, 300);
            group.spacing = new Vector2(5, 0);
            group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            group.startAxis = GridLayoutGroup.Axis.Horizontal;
            group.childAlignment = TextAnchor.UpperLeft;
            group.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            group.constraintCount = 1;
            group.enabled = false;

            sr.content = rectTransformContent;

            //Need to add example child components like in the Asset (SJ)
            Selection.activeGameObject = horizontalTableViewRoot;
        }

        [MenuItem("GameObject/UI/Vertical TableView", false)]
        static void AddVerticalTableView(MenuCommand menuCommand)
        {
            GameObject verticalTableViewRoot = CreateUIElementRoot("vertical_tableview", menuCommand, s_ThickGUIElementSize);

            GameObject childTableRect = CreateUIObject("tablerect", verticalTableViewRoot);

            GameObject childContent = CreateUIObject("content", childTableRect);

            // Set RectTransform to stretch
            RectTransform rectTransformScrollSnapRoot = verticalTableViewRoot.GetComponent<RectTransform>();
            rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.pivot = new Vector2(0.5f, 0.5f);
            rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
            rectTransformScrollSnapRoot.sizeDelta = new Vector2(300f, 600f);

            Image image = verticalTableViewRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.type = Image.Type.Sliced;
            image.color = new Color(1f, 1f, 1f, 0.392f);

            verticalTableViewRoot.AddComponent<RectMask2D>();

            ScrollRect sr = verticalTableViewRoot.AddComponent<ScrollRect>();
            sr.vertical = true;
            sr.horizontal = false;

            RectTransform rectTransformRect = childTableRect.GetComponent<RectTransform>();
            rectTransformRect.anchorMin = Vector2.zero;
            rectTransformRect.anchorMax = new Vector2(1f, 1f);
            rectTransformRect.pivot = new Vector2(0.5f, 0.5f);
            rectTransformRect.sizeDelta = Vector2.zero;

            sr.viewport = rectTransformRect;

            //Setup Content container
            RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
            rectTransformContent.anchorMin = new Vector2(0.5f, 1f);
            rectTransformContent.anchorMax = new Vector2(0.5f, 1f);
            rectTransformContent.pivot = new Vector2(0.5f, 1f);
            rectTransformContent.sizeDelta = new Vector2(300f, 600f);

            GridLayoutGroup group = childContent.AddComponent<GridLayoutGroup>();
            group.cellSize = new Vector2(300,100);
            group.spacing = new Vector2(0, 5);
            group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            group.startAxis = GridLayoutGroup.Axis.Vertical;
            group.childAlignment = TextAnchor.UpperCenter;
            group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            group.constraintCount = 1;
            group.enabled = false;

            sr.content = rectTransformContent;

            //Need to add example child components like in the Asset (SJ)
            Selection.activeGameObject = verticalTableViewRoot;
        }

        private static GameObject CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }
            GameObject child = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(child, "Create " + name);
            Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
            GameObjectUtility.SetParentAndAlign(child, parent);

            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            if (parent != menuCommand.context) // not a context click, so center in sceneview
            {
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), rectTransform);
            }
            Selection.activeGameObject = child;
            return child;
        }
        
        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            GameObjectUtility.SetParentAndAlign(go, parent);
            return go;
        }

        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }
    }
}

