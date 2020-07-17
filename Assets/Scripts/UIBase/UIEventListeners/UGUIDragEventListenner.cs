//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIDragEventListenner : UGUIEventListener, IBeginDragHandler, IDragHandler,
                                  IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, IInitializePotentialDragHandler
{

    // Use this for initialization
    void Start()
    {

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onDrag");
        }
        if (this.onDrag != null)
        {
            this.onDrag(gameObject.name, eventData.delta, eventData.position);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("OnDrop");
        }
        if (this.onDrop != null)
        {
            this.onDrop(gameObject.name);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onBeginDrag");
        }
        if (this.onBeginDrag != null)
        {
            this.onBeginDrag(gameObject.name, eventData.delta, eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onEndDrag");
        }
        if (this.onEndDrag != null)
        {
            this.onEndDrag(gameObject.name, eventData.delta, eventData.position);
        }
    }

    public virtual void OnScroll(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onScroll");
        }
        if (this.onScroll != null)
        {
            this.onScroll(gameObject.name);
        }
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onUpdateSelected");
        }
        if (this.onUpdateSelected != null)
        {
            this.onUpdateSelected(gameObject.name);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onInitializePotentialDrag");
        }
        if (this.onInitializePotentialDrag != null)
        {
            this.onInitializePotentialDrag(gameObject.name);
        }
    }

}
