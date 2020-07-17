//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework
{
    /// <summary>
    /// ColaFramework框架输入管理器
    /// </summary>
    public class InputMgr : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKey(KeyCode.Escape))
            {
                ConfirmQuit();
            }
        }

        /// <summary>
        /// 退出游戏前确认对话框
        /// </summary>
        public void ConfirmQuit()
        {
            Application.Quit();
#if UNITY_ANDROID && !UNITY_EDITOR
#endif
        }

    }
}


