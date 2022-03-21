using UnityEngine;

namespace ET
{
    // 战斗用常量代码配置
    public static class ReferenceHelper
    {
        private static GameObject fishRootNode;
        public static GameObject FishRootNode => fishRootNode;

#if UNITY_EDITOR

        internal static GizmosCaller GizmosCaller;
#endif

        public static void Init()
        {
            fishRootNode = GameObject.Find(ConstHelper.FishRootNodeName);
#if UNITY_EDITOR

            if (!fishRootNode.TryGetComponent(out GizmosCaller))
                GizmosCaller = fishRootNode.AddComponent<GizmosCaller>();
#endif
        }
    }
}