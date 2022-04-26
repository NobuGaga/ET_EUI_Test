// Battle Review Before Boss Node

namespace ET
{
    /// <summary> 鱼屏幕数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用 </summary>
    public static class FishScreenInfoHelper
    {
        public static FishScreenInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.FishScreenInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(FishScreenInfo)) as FishScreenInfo;
            unit.FishScreenInfo = info;
            return info;
        }

        public static void PushPool(long unitId, FishScreenInfo info)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            if (unit != null)
                unit.FishScreenInfo = null;

            MonoPool.Instance.Recycle(info);
        }
    }
}