using UnityEngine;

namespace ET
{
    /// <summary> Mono 层自己运行的逻辑处理 </summary>
    public static class BattleLogic
    {
        /// <summary> Mono 层战斗初始化标识 </summary>
        private static bool isInit = false;

        public static void Init()
        {
            // 因为热更层调用该接口的组件可能多次调用, 例如挂载在 Current Scene 上则会
            // 所以引用初始化标识进行判断
            if (isInit) return;

            // 调用注意顺序, 先初始化 Mono 层引用类
            ReferenceHelper.Init();
            BulletCameraHelper.Init(Screen.width, Screen.height, ReferenceHelper.CannoCamera.orthographicSize);

#if UNITY_EDITOR

            ReferenceHelper.GizmosCaller += BattleDebug.OnDrawGizmosCallBack;
#endif
            isInit = true;
        }


        public static void Clear()
        {
#if UNITY_EDITOR

            BattleDebug.Clear();
#endif
        }
    }
}