using System;
using UnityEngine;

namespace ET
{
    /// <summary> Unit Mono 层 Transform 管理组件类 </summary>
    public static class TransformMonoHelper
    {
        public static void Add(long unitId, Transform transform)
        {
            var unit = UnitMonoComponent.Instance.Get(unitId);
            if (unit != null)
                unit.Transform = transform;
        }

        public static void Remove(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get(unitId);
            if (unit != null)
                unit.Transform = null;
        }
    }
}