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
    /// 检查功能开启结果枚举
    /// </summary>
    public enum CheckFuncResult : byte
    {
        /// <summary>
        /// 功能未开启
        /// </summary>
        False = 0,
        /// <summary>
        /// 功能开启
        /// </summary>
        True = 1,
        /// <summary>
        /// 功能未开启，时间限制
        /// </summary>
        LevelLimit = 2,
        /// <summary>
        /// 功能未开启,等级限制
        /// </summary>
        TimeLimit = 3,
        /// <summary>
        /// 未知原因，备用
        /// </summary>
        None = 4,
    }

    /// <summary>
    /// 常量数据定义 
    /// </summary>
    public static class Constants
    {
        public static Color ColorWhite = Color.white;
        public static Color ColorRed = Color.red;
        public static Color ColorGreen = Color.green;
        public static Color ColorBlue = Color.blue;
        public static Color ColorBlack = Color.black;
        public static Color ColorYellow = Color.yellow;
        public static Color ColorGray = Color.gray;
        public static Color ColorUIMask = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 125 / 255.0f);

        public static readonly string UIViewTag = "UIView";
        public static readonly string UIIgnoreTag = "UIIgnore";
        public static readonly string UIPropertyTag = "UIProperty";

        public static readonly string AssetRoot = "Assets/";
        public static readonly string GameAssetBasePath = "Assets/GameAssets/";
        public static readonly string RawAssetBasePath = "Assets/RawAssets/";
        public static readonly string AppearanceDataPath = "AppearanceData/";
        public static readonly string UIModelSettingPath = AppearanceDataPath + "UIModelSettingData/";
        public static readonly string ModelAnimClipsPath = "Arts/AnimationClips/";
        public static readonly string ModelAnimatorPath = "Arts/Animator/";
        public static readonly string LutifyTexturePath = "Arts/Luts/";

        public static readonly string UIExportPrefabReltaPath = "Arts/UI/Prefabs/";
        public static readonly string UIExportPrefabPath = GameAssetBasePath + UIExportPrefabReltaPath;
        public static readonly string UIExportCSScriptPath = "Assets/Scripts/_UIViews/";
        public static readonly string UIExportLuaViewPath = "Assets/Lua/UIBindViews/";
        public static readonly string UIExportLuaAtlasCfgPath = "Assets/Lua/Common/";
    }

    public class ABFileInfo
    {
        public string filename;
        public string md5;
        public float rawSize;         // 压缩前的文件大小
        public float compressSize;      // 压缩后的文件大小
    }
}
