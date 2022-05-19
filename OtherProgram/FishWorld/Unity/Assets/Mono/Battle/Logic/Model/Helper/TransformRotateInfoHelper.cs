// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用
    /// 用于处理复杂的鱼线计算
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class TransformRotateInfoHelper
    {
        public static TransformRotateInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.TransformRotateInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(TransformRotateInfo)) as TransformRotateInfo;
            unit.TransformRotateInfo = info;
            return info;
        }

        public static void PushPool(long unitId, TransformRotateInfo info)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            if (unit != null)
                unit.TransformRotateInfo = null;

            MonoPool.Instance.Recycle(info);
        }
    }
}