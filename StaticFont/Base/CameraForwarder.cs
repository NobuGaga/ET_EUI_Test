using UnityEngine;

namespace StaticFont.Extension
{
    /// <summary>
    /// 跟随面向摄像机组件
    /// </summary>
    [ExecuteInEditMode]
    public sealed class CameraForwarder : MonoBehaviour
    {

        private Transform m_forwardCameraTrans;

#if UNITY_EDITOR

        [SerializeField] private Camera m_forwardCamera;
#endif
        public Camera ForwardCamera
        {
            set => m_forwardCameraTrans = value != null ? value.transform : null;
        }

        [SerializeField] private Vector3 m_baseScale = new Vector3(0.001f, 0.001f, 0.001f);
        public Vector3 BaseScale
        {
            set => m_baseScale = value;
        }

        private float GetDistance()
        {
            float distanceX = transform.position.x - m_forwardCameraTrans.position.x;
            float distanceY = transform.position.y - m_forwardCameraTrans.position.y;
            float distanceZ = transform.position.z - m_forwardCameraTrans.position.z;

            return distanceX * m_forwardCameraTrans.forward.x + distanceY * m_forwardCameraTrans.forward.y +
                    distanceZ * m_forwardCameraTrans.forward.z;
        }

        void LateUpdate()
        {
            if (m_forwardCameraTrans == null)
                return;

            transform.forward = m_forwardCameraTrans.forward;
            transform.localScale = m_baseScale * GetDistance();
        }
    }
}