//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace ColaFramework
{
    public class GameSplashEntry : MonoBehaviour
    {
        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private GameObject SkipLabel;

        private bool isPlayEnd;
        private string videoPath;

        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_EDITOR
            videoPath = Application.dataPath + "/RawAssets/Videos/";
#else
            videoPath = Application.streamingAssetsPath + "/Videos/";
#endif
        }

        // Use this for initialization
        void Start()
        {
            videoPlayer.loopPointReached += onLoopPointReached;
            PlayVideo(AppConst.SplashVideoName);
        }

        // Update is called once per frame
        void Update()
        {
            if (!isPlayEnd)
            {
                if (Input.anyKeyDown)
                {
                    if (videoPlayer.url != GetFilePath(AppConst.SplashVideoName))
                    {
                        onPlayEnd();
                    }
                }
            }
        }

        private void onLoopPointReached(VideoPlayer player)
        {
            if (player.url == GetFilePath(AppConst.SplashVideoName))
            {
                SkipLabel.SetActive(true);
                PlayVideo(AppConst.StoryVideoName);
            }
            else if (player.url == GetFilePath(AppConst.StoryVideoName))
            {
                onPlayEnd();
            }
            else
            {
                Debug.LogError("播放闪屏视频错误!");
            }
        }

        private void PlayVideo(string name)
        {
            videoPlayer.Stop();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = GetFilePath(name);

            videoPlayer.isLooping = false;
            videoPlayer.Play();
        }

        private void onPlayEnd()
        {
            videoPlayer.Stop();
            isPlayEnd = true;
            Destroy(gameObject);
            SceneManager.LoadScene("GameLauncher");
        }

        private string GetFilePath(string name)
        {
            return string.Format("{0}{1}", videoPath, name);
        }
    }
}
