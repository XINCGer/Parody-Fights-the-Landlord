//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UGUI版的UIToggleObjects 用于控制Toggle显示/隐藏一组物体
/// </summary>
[RequireComponent(typeof(Toggle))]
public class UGUIToggleObjects : MonoBehaviour
{
    [Tooltip("Toggle激活时需要显示的物体列表")]
    public List<GameObject> activate;
    [Tooltip("Toggle激活时需要隐藏的物体列表")]
    public List<GameObject> deactivate;

    void Awake()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        Toggle toggle = GetComponent<Toggle>();
        OnToggle(toggle.isOn);
        toggle.onValueChanged.AddListener(OnToggle);
    }

    public void OnToggle(bool val)
    {
        if (enabled)
        {
            for (int i = 0; i < activate.Count; ++i)
                Set(activate[i], val);

            for (int i = 0; i < deactivate.Count; ++i)
                Set(deactivate[i], !val);
        }
    }

    void Set(GameObject go, bool state)
    {
        if (go != null)
        {
            go.SetActive(state);
        }
    }
}
