// Battle Review

using UnityEngine;

namespace ET 
{
    public class CircCollider : MonoBehaviour
    {   
        public float radio;
        public Vector3 position;

        public Color gizmosColor = Color.yellow;

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            string sceneName = Camera.current.name;
            if (sceneName == ColliderConfig.CannonCameraName ||
                sceneName == ColliderConfig.UICameraName)
                return;

            if (sceneName == ColliderConfig.SceneCameraName && Application.isPlaying)
                return;

            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, radio);
        }
#endif
    }
}