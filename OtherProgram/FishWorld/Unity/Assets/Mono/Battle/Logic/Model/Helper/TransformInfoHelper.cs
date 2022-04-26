// Battle Review Before Boss Node

namespace ET
{
    /// <summary> 变换组件数据 Mono 组件类, 对应热更层 TransformComponent 持有跟调用 </summary>
    public static class TransformInfoHelper
    {
        public static TransformInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get(unitId);
            var info = unit.TransformInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(TransformInfo)) as TransformInfo;
            unit.TransformInfo = info;
            return info;
        }

        public static void PushPool(long unitId, TransformInfo info)
        {
            var unit = UnitMonoComponent.Instance.Get(unitId);
            if (unit != null)
                unit.TransformInfo = null;

            MonoPool.Instance.Recycle(info);
        }
    }
}