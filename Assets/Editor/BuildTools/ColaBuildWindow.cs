//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using LitJson;
using ColaFramework.Foundation;

namespace ColaFramework.ToolKit
{

    /// <summary>
    /// 可视化打包窗口，方便在本地进行打包测试
    /// </summary>
    public class ColaBuildWindow : OdinEditorWindow
    {
        private static ColaBuildWindow window;
        private const string CDN_CACHE_PATH = "cdn_cfg.json";

        [LabelText("是否母包")]
        [SerializeField]
        [LabelWidth(200)]
        private bool isMotherPkg;

        [LabelText("是否热更")]
        [SerializeField]
        [LabelWidth(200)]
        private bool isHotUpdate;

        [LabelText("是否Development Debug包")]
        [LabelWidth(200)]
        [SerializeField]
        private bool isDevelopment = false;

        [LabelText("是否Mono包")]
        [LabelWidth(200)]
        [SerializeField]
        private bool isMono = false;

        [LabelText("C#宏定义")]
        [SerializeField]
        [LabelWidth(200)]
        private string CSSymbolDefine;

        [LabelText("是否上传远端CDN")]
        [SerializeField]
        [LabelWidth(200)]
        private bool isUpLoadRemoteCDN;

        [LabelText("远端CDN地址")]
        [SerializeField]
        [LabelWidth(200)]
        [ShowIf("isUpLoadRemoteCDN")]
        private string CDNURL;

        [LabelText("远端CDN用户名")]
        [SerializeField]
        [LabelWidth(200)]
        [ShowIf("isUpLoadRemoteCDN")]
        private string CDNUserName;

        [LabelText("远端CDN密码")]
        [SerializeField]
        [LabelWidth(200)]
        [ShowIf("isUpLoadRemoteCDN")]
        private string CDNPassword;

        [LabelText("是否分析Bundle")]
        [SerializeField]
        [LabelWidth(200)]
        private bool AnalyzeBundle = true;

        [LabelText("选择打包平台")]
        [SerializeField]
        [LabelWidth(200)]
        private BuildTarget BuildTarget;

        [Button("一键打包", ButtonSizes.Large, ButtonStyle.Box)]
        private void BuildPlayer()
        {
            try
            {
                var path = ColaEditHelper.TempCachePath + "/" + CDN_CACHE_PATH;
                var cdnInfo = new CDNInfo();
                cdnInfo.CDNURL = CDNURL;
                cdnInfo.CDNUserName = CDNUserName;
                cdnInfo.CDNPassword = CDNPassword;
                FileHelper.WriteString(path, JsonMapper.ToJson(cdnInfo));
            }
            catch (Exception ex)
            {
                Debug.LogError("尝试读取CDN配置时报错!" + ex.Message);
            }

            ColaBuildTool.SetEnvironmentVariable(EnvOption.MOTHER_PKG, isMotherPkg.ToString(), false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.HOT_UPDATE_BUILD, isHotUpdate.ToString(), false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.CS_DEF_SYMBOL, CSSymbolDefine, false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.DEVLOPMENT, isDevelopment.ToString(), false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.IS_MONO, isMono.ToString(), false);

            ColaBuildTool.SetEnvironmentVariable(EnvOption.REMOTE_CDN, isUpLoadRemoteCDN.ToString(), false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.CDN_URL, CDNURL, false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.CDN_USERNAME, CDNUserName, false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.CDN_PASSWORD, CDNPassword, false);

            ColaBuildTool.SetEnvironmentVariable(EnvOption.ANALYZE_BUNDLE, AnalyzeBundle.ToString(), false);

            ColaBuildTool.SetEnvironmentVariable(EnvOption.APP_NAME, "ColaFramework", false);
            var timeNow = DateTime.Now;
            var timeNowStr = string.Format("{0:d4}{1:d2}{2:d2}_{3:d2}{4:d2}{5:d2}", timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, timeNow.Second);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.BUILD_PATH, ColaEditHelper.ProjectRoot + "/Build/" + timeNowStr, false);

            ColaBuildTool.BuildPlayer(BuildTarget);
        }

        private void Init()
        {
            ColaBuildTool.ClearEnvironmentVariable();
            try
            {
                var path = ColaEditHelper.TempCachePath + "/" + CDN_CACHE_PATH;
                var content = FileHelper.ReadString(path);
                if (!string.IsNullOrEmpty(content))
                {
                    var cdnInfo = JsonMapper.ToObject<CDNInfo>(content);
                    if (null != cdnInfo)
                    {
                        CDNURL = cdnInfo.CDNURL;
                        CDNUserName = cdnInfo.CDNUserName;
                        CDNPassword = cdnInfo.CDNPassword;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("尝试读取CDN配置时报错!" + ex.Message);
            }
        }

        [MenuItem("Build/快速打包窗口", priority = 1)]
        public static void PopUp()
        {
            window = GetWindow<ColaBuildWindow>("快速打包窗口");
            window.maximized = false;
            window.maxSize = window.minSize = new Vector2(500, 300);
            window.Init();
            window.Show();
        }

        [System.Serializable]
        private class CDNInfo
        {
            public string CDNURL;
            public string CDNUserName;
            public string CDNPassword;
        }
    }
}
