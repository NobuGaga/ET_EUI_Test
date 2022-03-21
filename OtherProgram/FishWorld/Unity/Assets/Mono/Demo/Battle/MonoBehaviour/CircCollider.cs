using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishWorld 
{
    public class CircCollider : MonoBehaviour
    {   
        public float radio;
        // public float radius;
        public Transform parent;
        public Vector3 position;

        private float _sqrRadius = -1;


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

        public float SqrRadius
        {
            get
            {
                if (_sqrRadius < 0)
                    _sqrRadius = radius * radius;

                return _sqrRadius;
            }
        }

        private void Update()
        {
            position = transform.position;

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
        
        public bool CheakCollider( float posX , float posY , float posZ  ) 
        {   
            Vector3 point = Vector3.zero;
            point.x = posX;
            point.y = posY;
            point.z = posZ;
            bool isCollider = false;
            //if (CameraManager.Instance != null) 
            //{
            //    Vector3 pos    = CameraManager.Instance.mainCamera.WorldToScreenPoint(position);
            //    float distance = (pos.x - point.x) * (pos.x  - point.x  ) + (pos.y - point.y  ) * (pos.y - point.y );
            //    Vector3 pos1   = Vector3.zero;
            //    pos1.x = position.x + radius;
            //    pos1.y = position.y;
            //    pos1.z = position.z;
            //    Vector3 pos2   = CameraManager.Instance.mainCamera.WorldToScreenPoint(pos1);
            //    _sqrRadius = (pos.x - pos2.x) * (pos.x - pos2.x) + (pos.y - pos2.y) * (pos.y - pos2.y); 
            //    if ( distance < _sqrRadius ) 
            //    {
            //        isCollider = true;
            //    }
            //}
            return isCollider;
        }

        public Color gizmosColor = Color.yellow;
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;//为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireSphere(transform.position, radius);//使用center和radius参数，绘制一个线框球体。
        }
#endif
    }

}
