//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ColaFramework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ColaFramework.Foundation;

namespace UnityEngine.UI.Extensions
{
    public class DropdownControl : MonoBehaviour, IControl, IPointerClickHandler
    {
        private Dropdown dropdown;
        public Action<int> onValueChanged;

        void Start()
        {
            InitDropdown();
            if (null != dropdown)
            {
                dropdown.onValueChanged.RemoveAllListeners();
                dropdown.onValueChanged.AddListener(OnValueChanged);
            }
        }

        private void OnValueChanged(int index)
        {
            if (null != onValueChanged)
            {
                onValueChanged(index);
            }
        }

        public void SetIndex(int index)
        {
            InitDropdown();
            dropdown.value = index;
        }

        public int GetIndex()
        {
            InitDropdown();
            return dropdown.value;
        }

        public void SetCaptionText(string text)
        {
            InitDropdown();
            dropdown.captionText.text = text;
        }

        public void RefreshShownValue()
        {
            dropdown.RefreshShownValue();
        }

        public void AddDropdownItem(string text, string imagePath = "")
        {
            InitDropdown();

            var data = new Dropdown.OptionData();
            data.text = text;
            if (!string.IsNullOrEmpty(imagePath))
            {
                var sprite = CommonUtil.AssetTrackMgr.GetAsset<Sprite>(imagePath);
                if (null != sprite)
                {
                    data.image = sprite;
                }
            }
            dropdown.options.Add(data);
        }

        public void ClearDropdownItem()
        {
            InitDropdown();
            dropdown.options.Clear();
        }

        public void Show()
        {
            InitDropdown();
            dropdown.Show();
        }

        void OnDestroy()
        {
            if (null != dropdown)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }
            onValueChanged = null;
        }

        private void InitDropdown()
        {
            if (null == dropdown)
            {
                dropdown = transform.GetComponent<Dropdown>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}