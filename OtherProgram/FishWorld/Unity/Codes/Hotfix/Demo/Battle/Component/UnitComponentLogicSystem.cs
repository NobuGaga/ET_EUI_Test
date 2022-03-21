using System.Collections.Generic;

namespace ET
{
    /// <summary> 原 UnitComponent 组件数据逻辑拓展 </summary>
	public static class UnitComponentLogicSystem
    {
        public static Unit AddFish(this UnitComponent self, UnitInfo unitInfo)
        {
            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId, BattleTestConfig.IsUseModelPool);
            // UnitFactory 会在 Add 后赋值, 这里怕有一些奇怪的逻辑前置赋值了
            unit.UnitType = unitInfo.UnitType;
            unit.AddComponent<BattleUnitLogicComponent, UnitInfo>(unitInfo, BattleTestConfig.IsUseModelPool);
            return unit;
        }

        public static HashSet<Unit> GetFishList(this UnitComponent self) => self.GetTypeUnits(UnitType.Fish);
    }
}