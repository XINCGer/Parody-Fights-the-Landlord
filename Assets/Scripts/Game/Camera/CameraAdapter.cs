//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI相机适配器,多出屏幕部分给黑边
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAdapter : MonoBehaviour
{
    /// <summary>
    /// UI相机视口比例
    /// </summary>
    public float Aspect
    {
        get
        {
            return aspect;
        }
        set
        {
            if (value > 0f)
            {
                aspect = value;
            }
            RefreshCameraRect();
        }
    }

    /// <summary>
    /// UI相机视口比例
    /// </summary>
    private float aspect = 16 / (float)9;

    private bool defaultExcute = false;

    private int defaultMask = 0;
    private Color defaultColor;
    public bool NeedUpdateRect = false;
    public Camera uiCamera;
    public bool IsMainCamera = false;
    [Header("勾选此选项宽度不加黑边")]
    public bool WidthAdapter = true;

    void Awake()
    {
        defaultExcute = false;
    }

    void Start()
    {
        SetRefreshCameraRect();
    }

    private void OnPostRender()
    {
        if (NeedUpdateRect && !IsMainCamera)
        {
            UpdateRect();
            NeedUpdateRect = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            RefreshCameraRect();
        }
    }

    private void SetRefreshCameraRect()
    {
        if (!defaultExcute)
        {
            RefreshCameraRect();
            defaultExcute = true;
        }
    }

    public void RefreshCameraRect()
    {
        if (uiCamera == null)
        {
            uiCamera = GetComponent<Camera>();
        }
        float aspectNow = Screen.width / (float)Screen.height;
        if (aspect > 0f && Mathf.Abs(aspectNow - aspect) > 0.01)
        {
            defaultMask = uiCamera.cullingMask;
            uiCamera.cullingMask = LayerMask.GetMask("Nothing");
            if (!defaultExcute)
            {
                defaultColor = uiCamera.backgroundColor;
            }
            uiCamera.backgroundColor = new Color(0, 0, 0, 1);
            if (gameObject.activeInHierarchy)
            {
                NeedUpdateRect = true;
                uiCamera.rect = new Rect(0, 0, 1, 1);
                uiCamera.Render();
                //uiCamera.RenderDontRestore();
            }

        }
        else
        {
            uiCamera.rect = new Rect(0, 0, 1, 1);
        }
    }


    public void UpdateRect()
    {
        int defaultScreenWith = Screen.width;
        int defaultScreenHeight = Screen.height;
        float aspectNow = defaultScreenWith / (float)defaultScreenHeight;
        float targetH = 1f;
        float targetV = 1f;
        if (aspectNow > aspect)
        {
            if (!WidthAdapter)
            {
                targetV = (defaultScreenHeight * aspect) / defaultScreenWith;
            }

        }
        else
        {
            targetH = defaultScreenWith / (defaultScreenHeight * aspect);
        }

        uiCamera.backgroundColor = defaultColor;
        uiCamera.cullingMask = defaultMask;
        if (targetH < 1f || targetV < 1f)//上下左右都切黑边
        //if (targetH < 1f)//只有上下切黑边，去掉左右切黑边
        {
            Rect rect = new Rect((1f - targetV) / 2f, (1f - targetH) / 2f, targetV, targetH);
            uiCamera.rect = rect;
        }
        else
        {
            uiCamera.pixelRect = new Rect(uiCamera.pixelRect.x,
            uiCamera.pixelRect.y, Screen.width, Screen.height);
            uiCamera.rect = new Rect(0, 0, 1, 1);
        }
    }
}
