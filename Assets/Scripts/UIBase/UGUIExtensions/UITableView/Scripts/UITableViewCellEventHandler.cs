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
    public class UITableViewCellEventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        internal GameObject targetObj;

        internal UITableViewCell tableViewCell;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (null != tableViewCell && null != tableViewCell.tableView)
            {
                tableViewCell.tableView.ProcessClick(tableViewCell, null == targetObj ? gameObject : targetObj);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (null != tableViewCell && null != tableViewCell.tableView)
            {
                tableViewCell.tableView.ProcessPress(true, tableViewCell, null == targetObj ? gameObject : targetObj);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (null != tableViewCell && null != tableViewCell.tableView)
            {
                tableViewCell.tableView.ProcessPress(false, tableViewCell, null == targetObj ? gameObject : targetObj);
            }
        }
    }
}