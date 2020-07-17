//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using ColaFramework;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 可拖动组件，适用于UI
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 offset;

        private Camera uiCamera;

        public delegate void OnBeginDragCallback(GameObject pointerDrag);
        public OnBeginDragCallback onBeginDragCallback;

        public delegate void OnDragingCallback(GameObject pointerDrag);
        public OnDragingCallback onDragingCallback;

        public delegate void OnDragEndCallback(GameObject pointerDrag, GameObject targetGo);
        public OnDragEndCallback onDragEndCallback;

        void Awake()
        {
            uiCamera = GUIHelper.GetUICamera();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 curTouchedWorldPos = uiCamera.ScreenToWorldPoint(eventData.position);
            offset = transform.position - curTouchedWorldPos;
            offset.z = 0;
            GetComponent<Image>().raycastTarget = false;
            SetDraggedPosition(eventData);
            if (onBeginDragCallback != null)
                onBeginDragCallback(eventData.pointerDrag);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetDraggedPosition(eventData);
            if (onDragingCallback != null)
                onDragingCallback(eventData.pointerDrag);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<Image>().raycastTarget = true;
            SetDraggedPosition(eventData);
            if (onDragEndCallback != null)
                onDragEndCallback(eventData.pointerDrag, eventData.pointerEnter);
        }

        private void SetDraggedPosition(PointerEventData eventData)
        {
            Vector3 worldpos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, uiCamera, out worldpos))
            {
                transform.position = worldpos + offset;
            }
        }
    }
}