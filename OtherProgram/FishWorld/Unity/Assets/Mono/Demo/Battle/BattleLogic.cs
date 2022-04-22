using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> Mono 层自己运行的逻辑处理 </summary>
    public static class BattleLogic
    {
        /// <summary> Mono 层战斗初始化标识 </summary>
        private static bool isInit = false;

        private static Dictionary<long, Transform> unitMap;

        public static void Init()
        {
            // 因为热更层调用该接口的组件可能多次调用, 例如挂载在 Current Scene 上则会
            // 所以引用初始化标识进行判断
            if (isInit) return;

            unitMap = new Dictionary<long, Transform>(ConstHelper.FisheryUnitCount);

            // 调用注意顺序, 先初始化 Mono 层引用类
            ReferenceHelper.Init();
            BulletCameraHelper.Init(Screen.width, Screen.height, ReferenceHelper.CannoCamera.orthographicSize);

#if UNITY_EDITOR

            ReferenceHelper.GizmosCaller += BattleDebug.OnDrawGizmosCallBack;
#endif
            isInit = true;
        }

        public static void Update()
        {
#if UNITY_EDITOR

            BattleDebug.Clear();
#endif
        }

        // 因为 Robot 工程不能使用 Unity 类, 所以视图层的 Mono 拓展放到这里
        #region TransformHelper Mono View Extension

        public static void Add(long unitId, Transform transform)
        {
            if (unitMap.ContainsKey(unitId))
                throw new Exception($"Mono unitMap already add unit. id = { unitId }");

            unitMap.Add(unitId, transform);
        }

        public static void Remove(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                throw new Exception($"Mono unitMap not exist unit. id = { unitId }");

            unitMap.Remove(unitId);
        }

        public static void SetLocalPos(long unitId, TransformInfo info)
        {
            if (!unitMap.ContainsKey(unitId))
                return;

            var transform = unitMap[unitId];
            transform.localPosition = info.LogicLocalPos;
            info.LogicPos = transform.position;
        }

        public static void SetForward(long unitId, TransformInfo info)
        {
            if (!unitMap.ContainsKey(unitId))
                return;

            unitMap[unitId].forward = info.LogicForward;
        }

        public static void Clear() => unitMap.Clear();

        #endregion
    }
}