//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 只是一个超级简单的用来调整高度的 未考虑各种情况和子节点重叠情况
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class AutoSize : MonoBehaviour, ILayoutGroup
{
    public bool autoVertical = true;
    public bool autoHorizontal = true;

    //private int childCount = 0;
    public RectTransform rectTransform;
    public LayoutGroup layoutGrop;
    public AutoSize parentAutoSize;
    private bool init = false;
	// Use this for initialization
    void Start()
	{
        if (!init)
        {
            Init();
            UpdateSize();
        }
	}

    private void Init()
    {
        init = true;
        if (layoutGrop == null)
        {
            layoutGrop = GetComponent<LayoutGroup>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        parentAutoSize =  transform.parent.GetComponent<AutoSize>();
    }

    public void UpdateSize()
    {
        float height = 0;
        float width = 0;
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var child = rectTransform.GetChild(i) as RectTransform;
            bool use = true;
            if (child != null && child.gameObject.activeSelf)
            {
                var fitter = child.gameObject.GetComponent<ContentSizeFitter>();
                if (fitter)
                {
                    if (autoHorizontal)
                    {
                        fitter.SetLayoutHorizontal();
                    }
                    if (autoVertical)
                    {
                        fitter.SetLayoutVertical();
                    }
                }
                var element = child.gameObject.GetComponent<ILayoutElement>();
                float addHeight = child.rect.height;
                float addWidth = child.rect.width;
                if (element!=null&&element.preferredHeight > child.rect.height)
                {
                    addHeight = element.preferredHeight;
                    addWidth = element.preferredWidth;
                }
                var layoutElement = child.gameObject.GetComponent<LayoutElement>();
                if (layoutElement)
                {
                    use = !layoutElement.ignoreLayout;
                }
                if (use)
                {
                    height += addHeight;
                    width += addWidth;
                }
     
            }
        }
        if (rectTransform.childCount == 0)
        {
            var element = gameObject.GetComponent<ILayoutElement>();
            float addHeight = rectTransform.rect.height;
            float addWidth = rectTransform.rect.width;
            if (element != null)
            {
                addHeight = element.preferredHeight;
                addWidth = element.preferredWidth;
            }
            height += addHeight;
            width += addWidth;
        }
        if (layoutGrop != null)
        {
            height = height + layoutGrop.padding.bottom + layoutGrop.padding.top;
            width = width + layoutGrop.padding.left + layoutGrop.padding.right;
        }
        if (autoHorizontal)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
        if (autoVertical)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        if (parentAutoSize!=null)
        {
            parentAutoSize.UpdateSize();
        }
        
    }

    public void SetLayoutHorizontal()
    {
#if UNITY_EDITOR
        if (!init)
        {
            Init();
        }
#endif
        if (init)
        {
            UpdateSize();
        }
       
    }

    public void SetLayoutVertical()
    {
#if UNITY_EDITOR
        if (!init)
        {
            Init();
        }
#endif
        if (init)
        {
            UpdateSize();
        }
    }
}
