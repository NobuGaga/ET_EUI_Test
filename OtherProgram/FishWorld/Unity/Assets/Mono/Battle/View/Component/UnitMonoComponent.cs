using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> Mono 层自己运行的逻辑处理 </summary>
    public static class UnitMonoComponent
    {
        private static Dictionary<long, Transform> unitMap;

        static UnitMonoComponent()
        {
            unitMap = new Dictionary<long, Transform>(ConstHelper.FisheryUnitCount);
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

        internal static void Dispose() => unitMap.Clear();

        #endregion
    }
}