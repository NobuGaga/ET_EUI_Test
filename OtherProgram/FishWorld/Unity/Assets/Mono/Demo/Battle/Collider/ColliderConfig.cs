using UnityEngine;

namespace ET
{
    public static class ColliderConfig
    {
        public const ushort FishCamera = 0;
        public const ushort CannonCamera = 1;

        public static Vector3 DefaultCenter = Vector3.zero;
        public const float DefaultScale = 1;
        
        public static Vector2 DefaultLineDirection = Vector2.up;
    }
}