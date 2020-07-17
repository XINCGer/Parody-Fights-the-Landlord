//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ColaFramework.ToolKit
{
    public static class JoystickGameObjectCreator
    {
        [MenuItem("GameObject/UI/VirtualJoystick")]
        static void CreateVirtualJoystick()
        {
            GameObject go = new GameObject("Joystick", typeof(VirtualJoystick));

            Canvas canvas = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Canvas>() : null;

            Selection.activeGameObject = go;

            if (!canvas)
                canvas = UnityEngine.Object.FindObjectOfType<Canvas>();

            if (!canvas)
            {
                canvas = new GameObject("Canvas", typeof(Canvas), typeof(RectTransform), typeof(GraphicRaycaster)).GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            if (canvas)
                go.transform.SetParent(canvas.transform, false);

            GameObject joystickGroup = new GameObject("JoystickGroup", typeof(RectTransform));
            joystickGroup.transform.SetParent(go.transform, false);

            GameObject background = new GameObject("Background", typeof(Image));
            GameObject graphic = new GameObject("Graphic", typeof(Image));

            background.transform.SetParent(joystickGroup.transform, false);
            graphic.transform.SetParent(joystickGroup.transform, false);

            background.GetComponent<Image>().color = new Color(1, 1, 1, .86f);

            RectTransform backgroundTransform = graphic.transform as RectTransform;
            RectTransform graphicTransform = graphic.transform as RectTransform;

            graphicTransform.sizeDelta = backgroundTransform.sizeDelta * .5f;

            VirtualJoystick joystick = go.GetComponent<VirtualJoystick>();
            joystick.JoystickGraphic = graphicTransform;
            joystick.JoystickGroup = joystickGroup.GetComponent<RectTransform>();
        }
    }
}