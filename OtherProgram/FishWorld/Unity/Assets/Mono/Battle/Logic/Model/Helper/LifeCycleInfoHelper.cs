// Battle Review Before Boss Node

namespace ET
{
    /// <summary>
    /// 鱼移动数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用
    /// 用于处理复杂的鱼线计算
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class LifeCycleInfoHelper
    {
        /// <summary> 使用服务器数据初始化存活数据 </summary>
        /// <param name="liveTime">出生时间戳(毫秒)</param>
        /// <param name="remainTime">剩余存活时间(秒)</param>
        public static void InitInfo(long unitId, long liveTime, int remainTime)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.LifeCycleInfo;
            info.SurvivalTime = (TimeHelper.ServerFrameTime() - liveTime) / 1000;
            info.TotalLifeTime = (float)remainTime / 1000 + info.SurvivalTime;
            info.CurrentLifeTime = info.SurvivalTime / info.TotalLifeTime;
        }

        public static void FixedUpdate(LifeCycleInfo lifeInfo, FishMoveInfo moveInfo)
        {
            if (lifeInfo.TotalLifeTime == 0)
                return;

            if (lifeInfo.IsLifeTimeOut)
            {
                moveInfo.StopMove();
                return;
            }
            
            lifeInfo.SurvivalTime += (float)TimeHelper.ClinetDeltaFrameTime() / 1000;
            lifeInfo.CurrentLifeTime = lifeInfo.SurvivalTime / lifeInfo.TotalLifeTime;
            if (lifeInfo.IsLifeTimeOut)
                moveInfo.StopMove();
        }

        public static float GetSurvivalTime(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.LifeCycleInfo;
            return info.SurvivalTime;
        }

        public static float GetTotalLifeTime(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.LifeCycleInfo;
            return info.TotalLifeTime;
        }
    }
}