using System.Collections.Generic;

namespace ET
{
    /// <summary> 原 UnitComponent 组件数据逻辑拓展 </summary>
	public static class UnitComponentLogicSystem
    {
        /// <summary> 获取玩家座位 ID </summary>
        /// <param name="playerId">玩家 ID</param>
        public static int GetSeatId(this UnitComponent self, long playerId)
        {
            Unit unit = self.Get(playerId);
            NumericComponent attributeComponent = unit.GetComponent<NumericComponent>();
            return attributeComponent.GetAsInt(NumericType.Pos);
        }
        

        public static Unit AddBattleUnit(this UnitComponent self, UnitInfo unitInfo)
        {
            bool isUseModelPool = BattleTestConfig.IsUseModelPool;

            int configId = unitInfo.ConfigId;
            if (unitInfo.UnitType == UnitType.Bullet)
                configId = BulletConfig.BulletUnitConfigId;

            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, configId, isUseModelPool);

            // UnitFactory 会在 Add 后赋值, 这里怕有一些奇怪的逻辑前置赋值了
            unit.UnitType = unitInfo.UnitType;

            unit.AddComponent<BattleUnitLogicComponent, UnitInfo>(unitInfo, isUseModelPool);
            return unit;
        }

        public static HashSet<Unit> GetFishUnitList(this UnitComponent self) => self.GetTypeUnits(UnitType.Fish);
    }
}