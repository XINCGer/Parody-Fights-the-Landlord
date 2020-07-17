//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.UI.Extensions;

//用来处理一些控件独有 不能统一处理的事件
public class OtherEventListenner : MonoBehaviour
{
    //InputField
    public StrValueChangeAction inputvalueChangeAction;
    public StrValueChangeAction inputeditEndAction;

    //Toggle
    public BoolValueChangeAction togglevalueChangeAction;
    //ScrollBar
    public FloatValueChangeAction scrollbarvalueChangeAction;
    //slider
    public FloatValueChangeAction slidervalueChangeAction;

    /// <summary>
    /// 新增触发事件回调，参数为触发的UI事件名称，比如onClick,onBoolValueChange,onSubmit等等
    /// </summary>
    public Action<string> onEvent;

    public void Awake()
    {
        //inputEditEndAction += delegate { };
        InputField input = gameObject.GetComponent<InputField>();
        if (input != null)
        {
            input.onValueChanged.AddListener(inputValueChangeHandler);
            input.onEndEdit.AddListener(inputEditEndHanler);
        }

        Toggle toggle = gameObject.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(toggleValueChangeHandler);
        }

        Scrollbar scrollbar = gameObject.GetComponent<Scrollbar>();
        if (scrollbar != null)
        {
            scrollbar.onValueChanged.AddListener(scrollbarValueChangeHandler);
        }

        Slider slider = gameObject.GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(sliderValueChangeHandler);
        }
    }

    private void inputValueChangeHandler(string text)
    {
        if (null != onEvent)
        {
            this.onEvent("onStrValueChange");
        }
        if (inputvalueChangeAction != null)
        {
            inputvalueChangeAction(gameObject.name, text);
        }
    }

    private void inputEditEndHanler(string text)
    {
        if (null != onEvent)
        {
            this.onEvent("onEditEnd");
        }
        if (inputeditEndAction != null)
        {
            inputeditEndAction(gameObject.name, text);
        }
    }

    private void toggleValueChangeHandler(bool select)
    {
        if (null != onEvent)
        {
            this.onEvent("onBoolValueChange");
        }
        if (togglevalueChangeAction != null)
        {
            togglevalueChangeAction(gameObject.name, select);
        }
    }

    private void sliderValueChangeHandler(float value)
    {
        if (null != onEvent)
        {
            this.onEvent("onFloatValueChange");
        }
        if (slidervalueChangeAction != null)
        {
            slidervalueChangeAction(gameObject.name, value);
        }
    }

    private void scrollbarValueChangeHandler(float value)
    {
        if (null != onEvent)
        {
            this.onEvent("onFloatValueChange");
        }
        if (scrollbarvalueChangeAction != null)
        {
            scrollbarvalueChangeAction(gameObject.name, value);
        }
    }

    public virtual void OnApplicationQuit()
    {
        this.inputvalueChangeAction = null;
        this.inputeditEndAction = null;
        this.togglevalueChangeAction = null;
        this.scrollbarvalueChangeAction = null;
        this.slidervalueChangeAction = null;
        this.onEvent = null;
    }
}

