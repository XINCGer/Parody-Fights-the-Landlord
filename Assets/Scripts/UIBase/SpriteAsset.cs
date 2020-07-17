//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 包含某个图集内的精灵资源的信息类(方便程序取用)
/// </summary>
[System.Serializable]
public class SpriteAsset : MonoBehaviour
{
    /// <summary>
    /// 存储某个图集内的所有精灵信息
    /// </summary>
    public List<SpriteAssetInfo> SpriteAssetInfos;

    /// <summary>
    /// 根据Sprite的name获取对应的Sprite
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Sprite GetSpriteByName(string name)
    {
        for (int i = 0; i < SpriteAssetInfos.Count; i++)
        {
            if (SpriteAssetInfos[i].name.Equals(name))
            {
                return SpriteAssetInfos[i].sprite;
            }
        }
        Debug.LogWarning(string.Format("没有找到Name为:{0}对应的Sprite!", name));
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("拆分图集为散图")]
    [LuaInterface.NoToLua]
    public void SplitTextureToPng()
    {
        if (SpriteAssetInfos.Count <= 0) return;
        var texture = SpriteAssetInfos[0].sprite.texture;
        var texturePath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
        importer.isReadable = true;
        AssetDatabase.ImportAsset(texturePath);

        for (int i = 0; i < SpriteAssetInfos.Count; i++)
        {
            EditorUtility.DisplayProgressBar(string.Format("拆分{0}", SpriteAssetInfos[i]), i.ToString(), i / SpriteAssetInfos.Count);
            Sprite sprite = SpriteAssetInfos[i].sprite;
            Texture2D altas = sprite.texture;
            Texture2D temp = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false);
            Color[] arrayColor = altas.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
            temp.SetPixels(arrayColor);
            temp.Apply();
            byte[] pixes = temp.EncodeToPNG();
            string path = Path.Combine(Application.dataPath, "tmpPngDir");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = sprite.name + ".png";

            File.WriteAllBytes(Path.Combine(path, fileName), pixes);
        }
        EditorUtility.ClearProgressBar();

        importer.isReadable = false;
        AssetDatabase.ImportAsset(texturePath);
        AssetDatabase.Refresh();
    }
#endif
}


/// <summary>
/// 单张图片/精灵的资源信息类
/// </summary>
[System.Serializable]
public class SpriteAssetInfo
{
    /// <summary>
    /// id
    /// </summary>
    public int id;

    /// <summary>
    /// 名称
    /// </summary>
    public string name;

    /// <summary>
    /// 精灵
    /// </summary>
    public Sprite sprite;
}
