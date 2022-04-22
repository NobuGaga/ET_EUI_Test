using UnityEngine;

namespace ET
{
    // 战斗节点引用辅助类
    public static class ReferenceHelper
    {
        public static Camera FishCamera { get; private set; }
        public static GameObject CannonCameraNode { get; private set; }
        public static Camera CannoCamera { get; private set; }
        public static CannonSeatLayout CannonMonoScript { get; private set; }
        public static GameObject BulletRootNode { get; private set; }

        public static GameObject FishRootNode { get; private set; }

#if UNITY_EDITOR

        internal static GizmosCaller GizmosCaller;
#endif

        public static void Init()
        {
            FishCamera = Camera.main;
            CannonCameraNode = GameObject.Find(ConstHelper.CannonCameraNodeName);
            CannoCamera = CannonCameraNode.GetComponent<Camera>();
            CannonMonoScript = CannonCameraNode.GetComponent<CannonSeatLayout>();
            BulletRootNode = CannonMonoScript.bulletPosTrans.gameObject;

            FishRootNode = GameObject.Find(ConstHelper.FishRootNodeName);

#if UNITY_EDITOR

            if (!FishRootNode.TryGetComponent(out GizmosCaller))
                GizmosCaller = FishRootNode.AddComponent<GizmosCaller>();
#endif
        }
    }
}