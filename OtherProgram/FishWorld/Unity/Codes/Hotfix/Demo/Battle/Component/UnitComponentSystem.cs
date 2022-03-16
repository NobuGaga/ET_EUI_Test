namespace ET
{
    /// <summary>
    /// 用来拓展原 UnitComponent
    /// </summary>
	public static class ExpandUnitComponentSystem
	{
        public static Unit AddFish(this UnitComponent self, UnitInfo unitInfo)
        {
            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId, BattleTestConfig.IsUseModelPool);
            // UnitFactory 会在 Add 后赋值, 这里怕有一些奇怪的逻辑前置赋值了
            unit.UnitType = (UnitType)unitInfo.Type;
            unit.AddComponent<BattleUnitLogicComponent, UnitInfo>(unitInfo, BattleTestConfig.IsUseModelPool);
            return unit;
        }
    }
}