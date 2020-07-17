//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ColaFramework.Foundation;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 音频资源自动压缩处理工具
    /// </summary>
    public class AudioPostProcessor : AssetPostprocessor
    {
        private const string AUDIO_PATH = "Assets/GameAssets/Audio/";
        private const string AUDIO_2D_PATH = "Assets/GameAssets/Audio/2d/";
        private const string AUDIO_3D_PATH = "Assets/GameAssets/Audio/3d/";

        /// <summary>
        /// 按照FileSize还是ClipLength划分
        /// </summary>
        private const bool DEVIDE_BY_SIZE = true;

        private const int SIZE_LEVEL_1 = 200 * 1024;
        private const int SIZE_LEVEL_2 = 1024 * 1024;

        private const int LENGTH_LEVEL_1 = 3;
        private const int LENGTH_LEVEL_2 = 15;

        private void OnPreprocessAudio()
        {
            if (assetPath.StartsWith(AUDIO_PATH))
            {
                var importer = (AudioImporter)assetImporter;

                if (assetPath.StartsWith(AUDIO_2D_PATH))
                {
                    importer.forceToMono = true;
                }
                else
                {
                    importer.forceToMono = false;
                }

                importer.preloadAudioData = true;
                importer.ambisonic = false;

                var setting = importer.defaultSampleSettings;
                if (DEVIDE_BY_SIZE)
                {
                    var fileSize = FileHelper.GetFileSize(assetPath);
                    if (fileSize <= SIZE_LEVEL_1)
                    {
                        setting.loadType = AudioClipLoadType.DecompressOnLoad;
                        importer.loadInBackground = false;
                    }
                    else if (fileSize <= SIZE_LEVEL_2)
                    {
                        setting.loadType = AudioClipLoadType.CompressedInMemory;
                        importer.loadInBackground = false;
                    }
                    else
                    {
                        setting.loadType = AudioClipLoadType.Streaming;
                        importer.loadInBackground = true;
                    }
                }
                else
                {
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
                    var audioLength = audioClip.length;
                    Resources.UnloadAsset(audioClip);

                    if (audioLength <= LENGTH_LEVEL_1)
                    {
                        setting.loadType = AudioClipLoadType.DecompressOnLoad;
                        importer.loadInBackground = false;
                    }
                    else if (audioLength <= LENGTH_LEVEL_2)
                    {
                        setting.loadType = AudioClipLoadType.CompressedInMemory;
                        importer.loadInBackground = false;
                    }
                    else
                    {
                        setting.loadType = AudioClipLoadType.Streaming;
                        importer.loadInBackground = true;
                    }

                }
                setting.compressionFormat = AudioCompressionFormat.Vorbis;
                setting.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                setting.quality = Mathf.Clamp(setting.quality, 0.5f, 0.7f);

                importer.ClearSampleSettingOverride("Android");
                importer.ClearSampleSettingOverride("iOS");
                importer.defaultSampleSettings = setting;
            }
        }
    }
}
