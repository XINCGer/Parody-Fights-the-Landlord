//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace ColaFramework
{
    public class SimpleMoveController : MonoBehaviour
    {

        Transform mainCamTransform;
        float h, v;
        Vector3 moveVec;
        CharacterController charCtrl;
        public float speed = 5;
        IAnimCtrl animCtrl;

        // Use this for initialization
        void Start()
        {

            mainCamTransform = GUIHelper.GetMainCamera().transform;
            charCtrl = GetComponent<CharacterController>();
        }

        public void Init(IAnimCtrl animCtrl)
        {
            this.animCtrl = animCtrl;
        }

        // Update is called once per frame
        void Update()
        {

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            moveVec = new Vector3(h, 0, v);

            if (h != 0 || v != 0)
            {
                animCtrl.PlayAnimation(AnimCurveEnum.Run);
                // 根据摄像机方向 进行移动
                moveVec = Quaternion.Euler(0, mainCamTransform.eulerAngles.y, 0) * moveVec;
                charCtrl.Move(moveVec * speed * Time.deltaTime);
                RotatePlayer();
            }
            else
            {
                animCtrl.PlayAnimation(AnimCurveEnum.Idle);
            }
        }

        private void RotatePlayer()
        {
            //向量v围绕y轴旋转cameraAngle.y度
            Quaternion qua = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Lerp(transform.rotation, qua, Time.deltaTime * 100);
        }
    }
}