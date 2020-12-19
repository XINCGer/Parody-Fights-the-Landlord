//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using ColaFramework.Foundation;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 大图/Sprite自动处理压缩编辑器
    /// </summary>
    public class TexturetPostprocessor : AssetPostprocessor
    {
        private const string ATLAS_PATH = "Assets/GameAssets/Arts/UI/Atlas/";
        private const string PICTURE_PATH = "Assets/GameAssets/Arts/UI/Pictures/";

        private static List<string> HIGH_Q_ATLAS = new List<string>()
        {
        };

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool genCode = false;

            foreach (string assetPath in importedAssets)
            {
                if (assetPath.StartsWith(ATLAS_PATH))
                {
                    genCode = true;
                    OnTextureImport(assetPath);
                }
                else if (assetPath.StartsWith(PICTURE_PATH))
                {
                    OnTextureImport(assetPath);
                }
            }
            foreach (string assetPath in deletedAssets)
            {
                if (assetPath.StartsWith(ATLAS_PATH))
                {
                    genCode = true;
                }
            }

            foreach (string assetPath in movedAssets)
            {
                if (assetPath.StartsWith(ATLAS_PATH))
                {
                    genCode = true;
                    OnTextureImport(assetPath);
                }
                else if (assetPath.StartsWith(PICTURE_PATH))
                {
                    OnTextureImport(assetPath);
                }
            }
            if (genCode)
            {
                GenLuaCode();
            }
        }

        void OnPreprocessTexture()
        {
            OnTextureImport(assetPath);
        }

        private static void OnTextureImport(string assetPath)
        {
            if (assetPath.StartsWith(ATLAS_PATH))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter != null)
                {
                    var dirName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
                    bool isHighQ = HIGH_Q_ATLAS.Contains(dirName);
                    ProcessTextureImport(textureImporter, "Standalone", dirName, true, isHighQ);
                    ProcessTextureImport(textureImporter, "iPhone", dirName, true, isHighQ);
                    ProcessTextureImport(textureImporter, "Android", dirName, true, isHighQ);
                }
            }
            else if (assetPath.StartsWith(PICTURE_PATH))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter != null)
                {
                    ProcessTextureImport(textureImporter, "Standalone", "", false, false);
                    ProcessTextureImport(textureImporter, "iPhone", "", false, false);
                    ProcessTextureImport(textureImporter, "Android", "", false, false);
                }
            }
        }

        public static void ProcessTextureImport(TextureImporter textureImporter, string platform, string packingTag, bool useAlpha, bool useHighQuality)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePixelsPerUnit = 100;
            textureImporter.spritePackingTag = packingTag;
            textureImporter.spriteImportMode = SpriteImportMode.Single;

            var importSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(importSettings);
            importSettings.spriteMeshType = SpriteMeshType.FullRect;
            importSettings.spriteExtrude = 1;
            importSettings.spriteAlignment = (int)SpriteAlignment.Center;
            importSettings.spriteGenerateFallbackPhysicsShape = false;
            textureImporter.SetTextureSettings(importSettings);

            textureImporter.sRGBTexture = true;
            textureImporter.alphaSource = useAlpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;
            textureImporter.alphaIsTransparency = true;
            textureImporter.isReadable = false;
            textureImporter.mipmapEnabled = false;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.crunchedCompression = false;
            textureImporter.compressionQuality = 100;

            var platformTextureSetting = textureImporter.GetPlatformTextureSettings(platform);
            platformTextureSetting.overridden = true;
            platformTextureSetting.maxTextureSize = 2048;
            platformTextureSetting.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;

            if (platform == "Standalone")
            {
                platformTextureSetting.format = useAlpha ?
                    (useHighQuality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.DXT5) :
                    (useHighQuality ? TextureImporterFormat.RGB24 : TextureImporterFormat.DXT1);
            }
            else if (platform == "iPhone")
            {
                platformTextureSetting.format = useAlpha ?
                    (useHighQuality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ASTC_RGBA_6x6) :
                    (useHighQuality ? TextureImporterFormat.RGB24 : TextureImporterFormat.ASTC_RGB_6x6);
            }
            else if (platform == "Android")
            {
                platformTextureSetting.format = useAlpha ?
                    (useHighQuality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ASTC_RGBA_6x6) :
                    (useHighQuality ? TextureImporterFormat.RGB24 : TextureImporterFormat.ASTC_RGB_6x6);
            }
            else
            {
                platformTextureSetting.format = useAlpha ?
                    (useHighQuality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGBA32) :
                    (useHighQuality ? TextureImporterFormat.RGB24 : TextureImporterFormat.RGB24);
            }

            platformTextureSetting.crunchedCompression = false;
            platformTextureSetting.compressionQuality = 100;
            platformTextureSetting.textureCompression = TextureImporterCompression.Compressed;
            platformTextureSetting.allowsAlphaSplitting = false;
            platformTextureSetting.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;

            textureImporter.SetPlatformTextureSettings(platformTextureSetting);
        }


        private static void GenLuaCode()
        {
            HashSet<string> alreadySpriteName = new HashSet<string>();
            var files = FileHelper.GetAllChildFiles(ATLAS_PATH, "png");

            StringBuilder sb = new StringBuilder(64);
            sb.AppendLine("--[[Notice:This lua atlasResPathConfig file is auto generate by TexturePostprocesser，don't modify it manually! --]]");
            sb.AppendLine();
            sb.AppendLine("local AtlasResPathCfg = {");

            foreach (var file in files)
            {
                if (!alreadySpriteName.Contains(file))
                {
                    alreadySpriteName.Add(file);
                }
                else
                {
                    Debug.LogError("有重名的Sprite文件！需要检查: " + file);
                }

                var fileName = Path.GetFileNameWithoutExtension(file);
                var filePath = FileHelper.FormatPath(file);
                filePath = filePath.Replace(Constants.GameAssetBasePath, "");
                sb.Append("\t").AppendFormat("['{0}'] = '{1}',", fileName, filePath).AppendLine();
            }
            sb.AppendLine("}");
            sb.AppendLine("return AtlasResPathCfg");

            FileHelper.WriteString(Constants.UIExportLuaAtlasCfgPath + "AtlasResPathCfg.lua", sb.ToString());
            AssetDatabase.Refresh();
        }
    }
}
