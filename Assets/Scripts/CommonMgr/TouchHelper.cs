//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ColaFramework框架点击触摸事件工具类
/// 一些在UI界面AttachListener的时候不会自动挂载事件监听器的GameObject
/// 可以在这里手动地添加监听器组件
/// </summary>
public class TouchHelper
{
    /// <summary>
    /// 单独为某一个物体添加点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="onClick"></param>
    public static void AddClickListener(GameObject go, UIEventHandler onClick)
    {
        if (null != go)
        {
            UGUIEventListener uGUIEventListener = go.AddSingleComponent<UGUIEventListener>();
            uGUIEventListener.isNeedCheckHideEvent = false;
            uGUIEventListener.onClick = onClick;
        }
    }

    /// <summary>
    /// 单独为某一个物体添加拖拽事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="onDrag"></param>
    public static void AddDragListener(GameObject go, UIDragEventHandlerDetail onDrag)
    {
        if (null != go)
        {
            UGUIDragEventListenner uGUIDragEventListenner = go.AddSingleComponent<UGUIDragEventListenner>();
            uGUIDragEventListenner.isNeedCheckHideEvent = false;
            uGUIDragEventListenner.onDrag = onDrag;
        }
    }

    /// <summary>
    /// 单独为某一个物体移除点击事件
    /// </summary>
    /// <param name="go"></param>
    public static void RemoveClickListener(GameObject go)
    {
        if (null != go)
        {
            UGUIEventListener uGUIEventListener = go.GetComponent<UGUIEventListener>();
            if (null != uGUIEventListener)
            {
                uGUIEventListener.onClick = null;
            }
        }
    }

    /// <summary>
    /// 单独为某一个物体移除拖拽事件
    /// </summary>
    /// <param name="go"></param>
    public static void RemoveDragListener(GameObject go)
    {
        if (null != go)
        {
            UGUIDragEventListenner uGUIDragEventListenner = go.AddSingleComponent<UGUIDragEventListenner>();
            if (null != uGUIDragEventListenner)
            {
                uGUIDragEventListenner.onDrag = null;
            }
        }
    }

    /// <summary>
    /// 单独为某一个物体添加拖拽结束事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="onEndDrag"></param>
    public static void AddEndDragListener(GameObject go, UIDragEventHandlerDetail onEndDrag)
    {
        if (null != go)
        {
            UGUIDragEventListenner uGUIDragEventListenner = go.AddSingleComponent<UGUIDragEventListenner>();
            if (null != uGUIDragEventListenner)
            {
                uGUIDragEventListenner.onEndDrag = onEndDrag;
            }
        }
    }

    /// <summary>
    /// 单独为某一个物体移除拖拽结束事件
    /// </summary>
    /// <param name="go"></param>
    public static void RemoveEndDragListener(GameObject go)
    {
        if (null != go)
        {
            UGUIDragEventListenner uGUIDragEventListenner = go.AddSingleComponent<UGUIDragEventListenner>();
            if (null != uGUIDragEventListenner)
            {
                uGUIDragEventListenner.onEndDrag = null;
            }
        }
    }
}
