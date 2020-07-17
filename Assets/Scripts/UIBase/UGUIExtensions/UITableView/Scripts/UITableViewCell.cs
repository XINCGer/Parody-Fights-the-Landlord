//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public class UITableViewCell : UITableViewCellEventHandler
    {
        [HideInInspector]
        public int index;
        [HideInInspector]
        public RectTransform cacheTransform;
        [HideInInspector]
        public GameObject cacheGameObject;

        internal UITableView tableView;

        void Awake()
        {
            cacheTransform = transform as RectTransform;
            cacheGameObject = this.gameObject;
            Debug.Assert(cacheTransform != null, "transform should be RectTransform");
            BindView();
        }

        private void BindView()
        {
            var selectables = GetComponentsInChildren<Selectable>(true);
            foreach (var item in selectables)
            {
                var evenHandler = item.gameObject.AddSingleComponent<UITableViewCellEventHandler>();
                evenHandler.targetObj = item.gameObject;
                evenHandler.tableViewCell = this;
            }
        }

        void OnDestroy()
        {
            index = -1;
            cacheTransform = null;
            cacheGameObject = null;
        }
    }
}