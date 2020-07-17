//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void UIEventHandler(string name);
public delegate void UIDragEventHandlerDetail(string name, Vector2 deltaPos, Vector2 curToucPosition);
public delegate void StrValueChangeAction(string name, string text);
public delegate void FloatValueChangeAction(string name, float value);
public delegate void IntValueChangeAction(string name, int value);
public delegate void BoolValueChangeAction(string name, bool isSelect);
public delegate void RectValueChangeAction(string name, Vector2 rect);

/// <summary>
/// UGUI事件的处理容器
/// </summary>
public interface IUGUIEventHandler
{
    /// <summary>
    /// 注册UIEventListener
    /// </summary>
    /// <param name="obj"></param>
    void AttachListener(GameObject obj);

    /// <summary>
    /// 反注册UIEventListener
    /// </summary>
    /// <param name="obj"></param>
    void UnAttachListener(GameObject obj);

    /// <summary>
    /// 移除GameObject上面的EventListener
    /// </summary>
    /// <param name="obj"></param>
    void RemoveEventHandler(GameObject obj);
}
