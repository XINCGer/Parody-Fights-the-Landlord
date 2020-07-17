//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	/// <summary>
	/// 在屏幕旋转后，自动调节 CanvasScaler 上的 referenceResolution
	/// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    [ExecuteInEditMode]
    [AddComponentMenu("Layout/Canvas Scaler Rotator", 101)]
	public class CanvasScalerRotator : UIBehaviour
	{
		private CanvasScaler m_CanvasScaler;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_CanvasScaler = GetComponent<CanvasScaler>();
            Handle();
        }

		void Update()
		{
			Handle();
		}

        protected virtual void Handle()
        {
            if (m_CanvasScaler == null)
                return;

            Vector2 referenceResolution = m_CanvasScaler.referenceResolution;
			if (referenceResolution.x == referenceResolution.y)
				return;

			if ((Screen.width > Screen.height) != (referenceResolution.x > referenceResolution.y))
			{
				m_CanvasScaler.referenceResolution = new Vector2(referenceResolution.y, referenceResolution.x);
			}
        }
	}
}
