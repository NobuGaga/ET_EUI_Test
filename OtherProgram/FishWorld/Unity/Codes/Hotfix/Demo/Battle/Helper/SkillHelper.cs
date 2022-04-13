namespace ET
{
    public static class SkillHelper
    {
        /// <summary> 获取追踪鱼 Unit 只有通过合法性检测才会返回非空值</summary>
        public static Unit GetTrackFishUnit(Scene currentScene, long trackFishUnitId)
        {
            if (trackFishUnitId == BulletConfig.DefaultTrackFishUnitId)
                return null;

            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);

            if (fishUnit != null && !fishUnit.IsDisposed && 
                fishUnit.GetComponent<TransformComponent>().IsInScreen)
                return fishUnit;

            return null;
        }
    }
}