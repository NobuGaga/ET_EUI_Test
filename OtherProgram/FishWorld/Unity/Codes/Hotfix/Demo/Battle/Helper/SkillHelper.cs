namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class SkillHelper
    {
        /// <summary> 获取追踪鱼 Unit 只有通过合法性检测才会返回非空值</summary>
        public static Unit GetTrackFishUnit(long trackFishUnitId)
        {
            if (trackFishUnitId == ConstHelper.DefaultTrackFishUnitId)
                return null;

            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
            Unit fishUnit = unitComponent.Get(trackFishUnitId);

            if (fishUnit != null && !fishUnit.IsDisposed && fishUnit.FishUnitComponent.ScreenInfo.IsInScreen)
                return fishUnit;

            return null;
        }
    }
}