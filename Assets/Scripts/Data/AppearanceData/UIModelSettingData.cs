//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UIModelSetting", menuName = "ColaFramework/UIModelSettingData", order = 1)]
public class UIModelSettingData : ScriptableObject
{
    [Tooltip("模型文件资源的相对路径")]
    public string modelResPath;

    public float cameraPitch = 0;
    public float cameraYaw = 90;
    public float cameraDistance = 7;
    public float cameraHeightOffset = 0.0f;
    public float modelCameraDepth = 6;
    public float positionX = 0;
    public float positionZ = 0;
}
