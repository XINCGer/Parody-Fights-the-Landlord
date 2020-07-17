//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 下拉选择框DropDown拓展版
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/DropdownExtension")]
    public class DropdownExtension : Dropdown
    {
        public Image openMark;
        public Image closeMark;

        [Serializable]
        public class DropListShowEvent : UnityEvent<bool> { }

        public DropListShowEvent onShowDropList = new DropListShowEvent();

        protected override void Awake()
        {
            base.Awake();
            ShowDropList(false);
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            GameObject dropList = base.CreateDropdownList(template);
            //Debug.Log("CreateDropdownList");
            ShowDropList(true);
            return dropList;
        }


        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            //Debug.Log("DestroyDropdownList");
            ShowDropList(false);
        }

        private void ShowDropList(bool show)
        {
            if (openMark)
            {
                openMark.gameObject.SetActive(show);
            }
            if (closeMark)
            {
                closeMark.gameObject.SetActive(!show);
            }
            onShowDropList.Invoke(show);
        }
    }
}