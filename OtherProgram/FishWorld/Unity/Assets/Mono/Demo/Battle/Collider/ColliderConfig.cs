using UnityEngine;

namespace ET
{
    public static class ColliderConfig
    {
        public const ushort FishCamera = 0;
        public const ushort CannonCamera = 1;

        public const string FishCameraName = "MainCamera";
        public const string CannonCameraName = "CannonCamera";
        public const string UICameraName = "UICamera";

#if UNITY_EDITOR

        public const string SceneCameraName = "SceneCamera";
#endif
        public static Vector3 DefaultCenter = Vector3.zero;
        public const float DefaultScale = 1;
        
        public static Vector2 DefaultLineDirection = Vector2.up;
    }
}