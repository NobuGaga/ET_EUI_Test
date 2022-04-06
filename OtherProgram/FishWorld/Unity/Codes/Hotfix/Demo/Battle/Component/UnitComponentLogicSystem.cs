using System.Collections.Generic;

namespace ET
{
    /// <summary> 原 UnitComponent 组件数据逻辑拓展 </summary>
	public static class UnitComponentLogicSystem
    {
        public static HashSet<Unit> GetPlayerUnitList(this UnitComponent self) => self.GetTypeUnits(UnitType.Player);

        public static Unit AddBattleUnit(this UnitComponent self, UnitInfo unitInfo)
        {
            bool isUseModelPool = BattleTestConfig.IsUseModelPool;

            int configId = unitInfo.ConfigId;
            if (unitInfo.UnitType == UnitType.Bullet)
                configId = BulletConfig.BulletUnitConfigId;

            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, configId, isUseModelPool);

            // Add BattleUnitLogicComponent 前要对 UnitType 进行赋值
            unit.UnitType = unitInfo.UnitType;

            unit.AddComponent<BattleUnitLogicComponent, UnitInfo, CannonShootInfo>(unitInfo, null, isUseModelPool);
            return unit;
        }

        public static HashSet<Unit> GetFishUnitList(this UnitComponent self) => self.GetTypeUnits(UnitType.Fish);

        public static Unit GetMaxScoreFish(this UnitComponent self)
        {
            HashSet<Unit> fishUnitList = self.GetFishUnitList();
            if (fishUnitList.Count <= 0)
                return null;

            int maxScore = 0;
            Unit maxScoreUnit = null;
            foreach (Unit fishUnit in fishUnitList)
            {
                TransformComponent transformComponent = fishUnit.GetComponent<TransformComponent>();
                if (!transformComponent.IsInScreen)
                    continue;

                var attributeComponent = fishUnit.GetComponent<NumericComponent>();
                int score = attributeComponent.GetAsInt(NumericType.Score);
                if (score > maxScore)
                {
                    maxScore = score;
                    maxScoreUnit = fishUnit;
                }
            }

            return maxScoreUnit;
        }
    }
}