// Battle Review

using UnityEngine;

namespace ET 
{
    public class CircCollider : MonoBehaviour
    {   
        public float radio;
        public Transform parent;
        public Vector3 position;

        private void Start() 
        {

        }

        public float radius
        {
            get
            {
                return radio * GetParentScale();
            }
        }

        private float GetParentScale()
        {
            float r = 0.1f;

            if(parent != null)
            {
               r = parent.localScale.x;
            }

            return r;
        }

        public Color gizmosColor = Color.yellow;

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (Camera.current.name != ColliderConfig.FishCameraName)
                return;

            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}