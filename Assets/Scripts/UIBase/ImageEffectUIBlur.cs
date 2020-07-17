//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using ColaFramework;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 根据相机照射的内容实时生成快速模糊的UI背景
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [ImageEffectAllowedInSceneView]
    public class ImageEffectUIBlur : MonoBehaviour
    {
        /// <summary>
        /// 渲染效果所用到的Shader
        /// </summary>
        private Shader effectShader;

        public bool EnableUIBlur = false;

        private bool isOpen = false;

        /// <summary>
        /// 最终渲染出来的临时Texture效果图
        /// </summary>
        private RenderTexture finalTexture = null;

        /// <summary>
        /// 主相机
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// 渲染效果所用到的材质
        /// </summary>
        private Material material;

        private Camera renderCamera;

        /// <summary>
        /// 最终渲染出来的临时Texture效果图
        /// </summary>
        public RenderTexture FinalTexture
        {
            get
            {
                bool state = false;
                if (false == EnableUIBlur)
                {
                    StartCoroutine(DelayInvoke(() =>
                    {
                        if (state)
                        {
                            if (isOpen)
                            {
                                mainCamera.enabled = false;
                            }
                        }
                    }, 0.1f));
                }
                EnableUIBlur = true;
                isOpen = true;
                state = true;
                renderCamera.Render();
                renderCamera.enabled = false;
                return finalTexture;
            }
            set
            {
                if (null == value)
                {
                    mainCamera.enabled = true;
                    EnableUIBlur = false;
                    isOpen = false;
                    //RenderTexture.ReleaseTemporary(finalTexture);
                    //finalTexture = null;
                }
            }
        }

        /// <summary>
        /// 渲染效果所用到的Shader
        /// </summary>
        public Shader EffecShader
        {
            get
            {
                if (null == effectShader)
                {
                    effectShader = Shader.Find("ColaFrameWork/MobileUIBlur");
                }
                return effectShader;
            }
        }

        /// <summary>
        /// 渲染效果所用到的材质
        /// </summary>
        public Material EffectMaterial
        {
            get
            {
                if (null == material)
                {
                    material = new Material(EffecShader);
                    material.hideFlags = HideFlags.HideAndDontSave;
                }
                return material;
            }
        }

        void Awake()
        {
            mainCamera = GUIHelper.GetMainCamera();
            renderCamera = gameObject.GetComponent<Camera>();
        }


        // Use this for initialization
        void Start()
        {
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                Debug.LogWarning("该设备上不支持ImageEffects！");
                return;
            }
            if (!effectShader || !effectShader.isSupported)
            {
                enabled = false;
                return;
            }
            enabled = true;
            EnableUIBlur = false;
        }

        void OnApplicationQuit()
        {
            if (material)
            {
                DestroyImmediate(material);
                RenderTexture.ReleaseTemporary(finalTexture);
                finalTexture = null;
            }
        }

        void OnDisable()
        {
            //if (material)
            //{
            //    DestroyImmediate(material);
            //    RenderTexture.ReleaseTemporary(finalTexture);
            //    finalTexture = null;
            //}
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (EnableUIBlur)
            {
                if (null == finalTexture)
                {
                    finalTexture = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0,
                        RenderTextureFormat.Default);
                }
                RenderTexture tempRenderTexture = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, RenderTextureFormat.Default);
                Graphics.Blit(source, tempRenderTexture, EffectMaterial, 0);
                Graphics.Blit(tempRenderTexture, finalTexture, EffectMaterial, 1);
                RenderTexture.ReleaseTemporary(tempRenderTexture);
                EnableUIBlur = false;
            }
        }

        public static IEnumerator DelayInvoke(Action action, float interval)
        {
            yield return new WaitForSeconds(interval);
            action();
        }
    }
}