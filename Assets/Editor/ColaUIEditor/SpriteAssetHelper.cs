//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColaFramework.ToolKit
{
    public static class SpriteAssetHelper
    {
        [MenuItem("Assets/Create Or Update Sprite Asset")]
        public static void Execute()
        {
            Object obj = Selection.activeObject;
            CreateOrUpdateSpriteAsset(obj, null);
        }

        /// <summary>
        /// 创建或者更新一个图集的资源信息
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="fullFileName"></param>
        public static void CreateOrUpdateSpriteAsset(Object targetObj, string fullFileName)
        {
            if (null == targetObj || targetObj.GetType() != typeof(Texture2D))
            {
                return;
            }
            Texture2D texture2D = targetObj as Texture2D;

            //获取图集的完整路径
            if (string.IsNullOrEmpty(fullFileName))
            {
                fullFileName = AssetDatabase.GetAssetPath(texture2D);
            }
            //截取带后缀的文件名
            string fileNameWithExtension = Path.GetFileName(fullFileName);
            //不带后缀名的文件名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullFileName);
            //不带文件名的纯路径
            string filePath = fullFileName.Replace(fileNameWithExtension, "");

            string assetPath = string.Format("{0}{1}.prefab", filePath, fileNameWithoutExtension);
            SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpriteAsset)) as SpriteAsset;
            if (null == spriteAsset)
            {
                GameObject prefab = null;
                GameObject tempObj = new GameObject(fileNameWithoutExtension);
                prefab = PrefabUtility.CreatePrefab(assetPath, tempObj);
                spriteAsset = prefab.AddComponent<SpriteAsset>();
                GameObject.DestroyImmediate(tempObj);
            }
            spriteAsset.SpriteAssetInfos = GetSpriteAssetInfos(texture2D);
            EditorUtility.SetDirty(spriteAsset.gameObject);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 获取一张Texture里面的所有精灵信息
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static List<SpriteAssetInfo> GetSpriteAssetInfos(Texture2D texture2D)
        {
            List<SpriteAssetInfo> spriteAssetInfos = new List<SpriteAssetInfo>();
            string filePath = AssetDatabase.GetAssetPath(texture2D);
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(filePath);

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetType() == typeof(Sprite))
                {
                    SpriteAssetInfo spriteAssetInfo = new SpriteAssetInfo();
                    Sprite sprite = objs[i] as Sprite;
                    spriteAssetInfo.id = i;
                    spriteAssetInfo.name = sprite.name;
                    spriteAssetInfo.sprite = sprite;
                    spriteAssetInfos.Add(spriteAssetInfo);
                }
            }
            return spriteAssetInfos;
        }
    }
}