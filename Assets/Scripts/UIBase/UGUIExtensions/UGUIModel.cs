//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using ColaFramework;
using ColaFramework.Foundation;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// UGUIMODEL组件，用来展示3D人物形象
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(EmptyRaycast))]
    public class UGUIModel : UIBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {

        #region 属性字段
        /// <summary>
        /// 模型的LayerName
        /// </summary>
        private const string UIModelLayerTag = "UI_Model";

        [SerializeField]
        [Tooltip("模型的缩放")]
        private Vector3 scale = Vector3.one;
        [SerializeField]
        [Tooltip("模型的X坐标")]
        private float positionX = 0.0f;
        [SerializeField]
        [Tooltip("模型的Z坐标")]
        private float positionZ = 0.0f;

        [SerializeField]
        [Tooltip("相机距离模型的距离")]
        private float cameraDistance = 3.0f;

        [SerializeField]
        [Tooltip("相机相对模型高度")]
        public float cameraHeightOffset = 0.0f;

        [SerializeField]
        [Tooltip("相机视野范围")]
        private int fieldOfView = 60;

        [SerializeField]
        [Tooltip("相机裁剪距离")]
        private int farClipPlane = 20;

        [SerializeField]
        [Tooltip("相机深度")]
        private float modelCameraDepth = 1;

        [Tooltip("相机X轴旋转参数")]
        [SerializeField]
        private float cameraPitch = 0.0f;

        [Tooltip("相机Y轴旋转参数")]
        [SerializeField]
        private float cameraYaw = 90;

        [SerializeField]
        [Tooltip("模型是否可以旋转")]
        private bool enableRotate = true;

        private GameObject root;
        private Camera uiCamera;
        private Camera modelCamera;
        private RectTransform rectTransform;
        private Transform modelRoot;
        private static Vector3 curPos = Vector3.zero;
        private Transform model;
        private int frameCount = 1;
        private bool isInitInEditor = false;

        private Vector3 tempRelaPosition = Vector3.zero;
        private Vector3 tempOffset = Vector3.zero;
        private Vector3[] screenCorners = new Vector3[4];

        //提前申请RaycatHit数组，避免频繁申请产生GC
        private RaycastHit[] hitInfos = new RaycastHit[20];

        /// <summary>
        /// UGUIModel内部维护模型数组,不在Lua中进行维护
        /// </summary>
        private IList<ISceneCharacter> modelList;
        private IDictionary<int, int> indexMap;

        public Transform Model
        {
            get { return model; }
            set
            {
                model = value;
                model.SetParent(modelRoot);
                frameCount = 1;
            }
        }

        private float ModelCameraDepth
        {
            get { return modelCameraDepth; }
            set
            {
                modelCameraDepth = value;
                modelCamera.depth = modelCameraDepth;
            }
        }

        /// <summary>
        /// 模型点击以后的回调函数
        /// </summary>
        public Action<string> onModelClick;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            isInitInEditor = false;
            uiCamera = GUIHelper.GetUICamera();
            rectTransform = this.GetComponent<RectTransform>();
            root = new GameObject("uguiModel");
            GameObject.DontDestroyOnLoad(root);
            root.transform.position = curPos;
            curPos += new Vector3(200, 0, 0);

            modelCamera = new GameObject("modelCamera", typeof(Camera)).GetComponent<Camera>();
            ModelCameraDepth = uiCamera.depth + 1.0f;
            modelCamera.cullingMask = LayerMask.GetMask(UIModelLayerTag);
            modelCamera.clearFlags = CameraClearFlags.Depth;
            modelCamera.fieldOfView = fieldOfView;
            modelCamera.farClipPlane = farClipPlane;
            modelCamera.transform.SetParent(root.transform);

            modelRoot = new GameObject("model_root").transform;
            modelRoot.transform.SetParent(root.transform);
            modelRoot.localPosition = Vector3.zero;
            modelRoot.localRotation = Quaternion.identity;

            modelList = new List<ISceneCharacter>();
            indexMap = new Dictionary<int, int>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (null != modelCamera)
            {
                modelCamera.enabled = true;
            }
            UpdateCameraEffect();
        }



        public void OnDrag(PointerEventData eventData)
        {
            if (enableRotate)
            {
                cameraYaw -= eventData.delta.x;
            }
        }

        private void OnClickModel()
        {
            //每次使用前清空结构体数组
            Array.Clear(hitInfos, 0, hitInfos.Length);
            Ray ray = modelCamera.ScreenPointToRay(Input.mousePosition);
            Physics.RaycastNonAlloc(ray, hitInfos, 100.0f, LayerMask.GetMask(UIModelLayerTag));
            for (int i = 0; i < hitInfos.Length; i++)
            {
                var hit = hitInfos[i];
                var collider = hit.collider;
                if (null != collider)
                {
                    var name = collider.name;
                    //if ("model_head" == name || "model_body" == "name" || "model_foot" == name)
                    //{
                    //}
                    if (null != onModelClick)
                    {
                        onModelClick(name);
                    }
                }
            }
        }

        /// <summary>
        /// 设置模型的整体的Layer(包括子节点)
        /// </summary>
        /// <param name="modelTrans"></param>
        private void SetModelLayer(Transform modelTrans)
        {
            foreach (var trans in modelTrans.GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = LayerMask.NameToLayer(UIModelLayerTag);
            }
        }

        private void Update()
        {
            if (null == modelCamera)
            {
                Debug.LogError("Error No ModelCamera!");
                return;
            }
            if (model)
            {
                model.localPosition = Vector3.zero;
                if (frameCount > 0)
                {
                    SetModelLayer(model);
                    frameCount--;
                }
            }
            //计算x,y,z的单位向量
            float y = Mathf.Sin(cameraPitch * Mathf.Deg2Rad);
            float x = Mathf.Cos(cameraYaw * Mathf.Deg2Rad);
            float z = Mathf.Sin(cameraYaw * Mathf.Deg2Rad);
            //对单位向量进行放大，拿到真实的世界坐标
            float radius = Mathf.Cos(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
            tempRelaPosition.Set(x * radius, y * cameraDistance, z * radius);
            tempOffset.Set(0, cameraHeightOffset, 0);
            modelCamera.transform.position = modelRoot.position + tempRelaPosition + tempOffset;
            Vector3 tempForward = modelRoot.position + tempOffset - modelCamera.transform.position;
            if (tempForward.sqrMagnitude >= 0)
            {
                modelCamera.transform.forward = tempForward;
            }

            modelRoot.localPosition = Vector3.zero;
            modelRoot.localRotation = Quaternion.identity;
            modelRoot.localScale = Vector3.one;
            rectTransform.GetWorldCorners(screenCorners);

            //适配UI
            //left botton corner of screen
            var screen_lb = uiCamera.WorldToScreenPoint(screenCorners[0]);
            //right top corner of screen
            var screen_rt = uiCamera.WorldToScreenPoint(screenCorners[2]);
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            float w = (screen_rt - screen_lb).x / screenWidth;
            float h = (screen_rt - screen_lb).y / screenHeight;
            modelCamera.rect = new Rect(screen_lb.x / screenWidth, screen_lb.y / screenHeight, w, h);

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (null != modelCamera)
            {
                modelCamera.enabled = false;
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null != root)
            {
                Array.Clear(hitInfos, 0, hitInfos.Length);
                hitInfos = null;
                for (int i = 0; i < modelList.Count; i++)
                {
                    if (null != modelList[i])
                    {
                        modelList[i].Release();
                    }
                }
                modelList.Clear();
                modelList = null;
                indexMap.Clear();
                indexMap = null;

                Destroy(root);
                root = null;
            }
            modelCamera = null;
            modelRoot = null;
            model = null;
            rectTransform = null;
            uiCamera = null;
            onModelClick = null;
        }

        public void SetCameraEffect(bool isEnable)
        {
            UpdateCameraEffect();
        }

        public bool IsModelExist(int index)
        {
            int realIndex;
            if (indexMap.TryGetValue(index, out realIndex))
            {
                if (realIndex < modelList.Count)
                {
                    return null != modelList[realIndex];
                }
            }
            return false;
        }

        public void SetModelAt(int index, ISceneCharacter sceneCharacter)
        {
            int realIndex;
            if (indexMap.TryGetValue(index, out realIndex))
            {
                modelList[realIndex] = sceneCharacter;
            }
            else
            {
                modelList.Add(sceneCharacter);
                indexMap[index] = modelList.Count - 1;
            }
        }

        public ISceneCharacter GetModelAt(int index)
        {
            int realIndex;
            if (indexMap.TryGetValue(index, out realIndex))
            {
                if (realIndex < modelList.Count)
                {
                    return modelList[realIndex];
                }
            }
            return null;
        }

        public void UpdateModelShownIndex(int index)
        {
            //先全部隐藏，然后再显示指定index的
            for (int i = 0; i < modelList.Count; i++)
            {
                if (null != modelList[i])
                {
                    modelList[i].Visible = false;
                }
            }
            var selectModel = GetModelAt(index);
            if (null != selectModel)
            {
                selectModel.Visible = true;
                Model = selectModel.transform;
            }
        }

        private void UpdateCameraEffect()
        {

        }

        public void ImportSetting(string settingName = "")
        {
            //读取序列化的配置文件，如果没有找到配置就使用默认配置
            string path = Constants.UIModelSettingPath + settingName + ".asset";
            if (string.IsNullOrEmpty(settingName))
            {
                DefaultSetting();
            }
            else
            {
                var modelData = CommonUtil.AssetTrackMgr.GetAsset<UIModelSettingData>(path);
                if (null == modelData)
                {
                    DefaultSetting();
                }
                else
                {
                    cameraPitch = modelData.cameraPitch;
                    cameraYaw = modelData.cameraYaw;
                    cameraDistance = modelData.cameraDistance;
                    cameraHeightOffset = modelData.cameraHeightOffset;
                    //暂时取消ModelCameraDepth读取配置，统一为UICameraDepth + 1
                    //ModelCameraDepth = modelData.modelCameraDepth;
                    ModelCameraDepth = uiCamera.depth + 1.0f;
                    positionX = modelData.positionX;
                    positionZ = modelData.positionZ;
                }
            }
        }


        [LuaInterface.NoToLua]
        public void DefaultSetting()
        {
            cameraPitch = 0;
            cameraYaw = 90;
            cameraDistance = 3;
            cameraHeightOffset = 0.0f;
            ModelCameraDepth = 7;
            positionX = 0;
            positionZ = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickModel();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }


        #region RunInEditor
#if UNITY_EDITOR
        [LuaInterface.NoToLua]
        [ExecuteInEditMode]
        public void UpdateInEditor(Vector2 screenSize)
        {
            if (null == modelCamera)
            {
                Debug.LogError("Error No ModelCamera!");
                return;
            }
            if (model)
            {
                //编辑器模式下直接设置模型的偏移量
                model.localPosition = new Vector3(positionX, 0, positionZ);
                if (frameCount > 0)
                {
                    SetModelLayer(model);
                    frameCount--;
                }
            }
            //计算x,y,z的单位向量
            float y = Mathf.Sin(cameraPitch * Mathf.Deg2Rad);
            float x = Mathf.Cos(cameraYaw * Mathf.Deg2Rad);
            float z = Mathf.Sin(cameraYaw * Mathf.Deg2Rad);
            //对单位向量进行放大，拿到真实的世界坐标
            float radius = Mathf.Cos(cameraPitch * Mathf.Deg2Rad) * cameraDistance;
            tempRelaPosition.Set(x * radius, y * cameraDistance, z * radius);
            tempOffset.Set(0, cameraHeightOffset, 0);
            modelCamera.transform.position = modelRoot.position + tempRelaPosition + tempOffset;
            Vector3 tempForward = modelRoot.position + tempOffset - modelCamera.transform.position;
            if (tempForward.sqrMagnitude >= 0)
            {
                modelCamera.transform.forward = tempForward;
            }

            modelRoot.localPosition = Vector3.zero;
            modelRoot.localRotation = Quaternion.identity;
            modelRoot.localScale = Vector3.one;
            rectTransform.GetWorldCorners(screenCorners);

            //适配UI
            //left botton corner of screen
            var screen_lb = uiCamera.WorldToScreenPoint(screenCorners[0]);
            //right top corner of screen
            var screen_rt = uiCamera.WorldToScreenPoint(screenCorners[2]);

            float w = (screen_rt - screen_lb).x / screenSize.x;
            float h = (screen_rt - screen_lb).y / screenSize.y;
            modelCamera.rect = new Rect(screen_lb.x / screenSize.x, screen_lb.y / screenSize.y, w, h);
        }

        [LuaInterface.NoToLua]
        [ExecuteInEditMode]
        public void ImportModelInEditor()
        {
            if (null != modelRoot)
            {
                var settingName = "";
                if (modelRoot.childCount > 0)
                {
                    model = modelRoot.GetChild(0);
                    frameCount = 1;
                    settingName = model.name;
                }
                else
                {
                    model = null;
                }

                ImportSetting(settingName);
            }
        }


        [LuaInterface.NoToLua]
        [ExecuteInEditMode]
        public void InitInEditor(Camera camera)
        {
            if (!gameObject.activeSelf || isInitInEditor)
            {
                return;
            }
            uiCamera = camera;
            rectTransform = transform as RectTransform;
            root = GameObject.Find("uguimodel_in_editor");
            if (null == root)
            {
                root = new GameObject("uguimodel_in_editor");
                root.transform.localRotation = Quaternion.identity;
            }
            root.transform.position = curPos;
            curPos += new Vector3(200, 0, 0);

            var modelCameraObj = GameObject.Find("model_camera_in_editor");
            if (null == modelCameraObj)
            {
                modelCameraObj = new GameObject("model_camera_in_editor", typeof(Camera));
            }
            modelCamera = modelCameraObj.GetComponent<Camera>();
            ModelCameraDepth = uiCamera.depth + 1.0f;
            modelCamera.cullingMask = LayerMask.GetMask(UIModelLayerTag);
            modelCamera.clearFlags = CameraClearFlags.Depth;
            modelCamera.fieldOfView = fieldOfView;
            modelCamera.farClipPlane = farClipPlane;
            modelCamera.transform.SetParent(root.transform);

            var modelRootObj = GameObject.Find("model_root_in_editor");
            if (null == modelRootObj)
            {
                modelRootObj = new GameObject("model_root_in_editor");
            }
            modelRoot = modelRootObj.transform;
            modelRoot.transform.SetParent(root.transform);
            modelRoot.localPosition = Vector3.zero;
            modelRoot.localRotation = Quaternion.identity;

            if (modelRoot.childCount > 0)
            {
                model = modelRoot.GetChild(0);
            }
            else
            {
                model = null;
            }
            isInitInEditor = true;
        }

        [LuaInterface.NoToLua]
        [ExecuteInEditMode]
        public void SaveSetting()
        {
            if (null != modelRoot && null != model)
            {
                var settingName = model.name;
                if (!string.IsNullOrEmpty(settingName))
                {
                    string fullPath = Constants.GameAssetBasePath + Constants.UIModelSettingPath + settingName + ".asset";
                    if (!File.Exists(fullPath))
                    {
                        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<UIModelSettingData>(), fullPath);
                    }
                    var setting = AssetDatabase.LoadAssetAtPath<UIModelSettingData>(fullPath);
                    setting.cameraPitch = cameraPitch;
                    setting.cameraYaw = cameraYaw;
                    setting.cameraDistance = cameraDistance;
                    setting.cameraHeightOffset = cameraHeightOffset;
                    setting.modelCameraDepth = modelCameraDepth;
                    setting.positionX = positionX;
                    setting.positionZ = positionZ;
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("错误提示", "模型节点名称不能为空字符！", "确定");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("错误提示", "模型节点不能为空！", "确定");
            }
        }
#endif
        #endregion
    }
}
