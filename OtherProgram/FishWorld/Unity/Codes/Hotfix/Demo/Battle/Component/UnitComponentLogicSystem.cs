// Battle Review Before Boss Node

using System.Collections.Generic;

using BattleLogicUnit = ET.BattleUnitLogicComponentSystem;

namespace ET
{
    /// <summary> 原 UnitComponent 组件数据逻辑拓展 </summary>
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(UnitComponent))]
	public static class UnitComponentLogicSystem
    {
        internal static HashSet<Unit> GetPlayerUnitList(this UnitComponent self) =>
                                      self.GetTypeUnits(UnitType.Player);

        internal static Unit AddFishUnit(this UnitComponent self, UnitInfo unitInfo)
        {
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId, isUseModelPool);

            // Add BattleUnitLogicComponent 前要对 UnitType 进行赋值
            unit.UnitType = UnitType.Fish;
            unit.Type = UnitType.Fish;

            unit.AddComponent<BattleUnitLogicComponent, UnitInfo>(unitInfo, isUseModelPool);

            // 处理完鱼的逻辑后, 判断数据合法性
            FishUnitComponent fishUnitComponent = unit.FishUnitComponent;
            if (!fishUnitComponent.Info.IsMoveEnd)
                return unit;

            self.RemoveChild(unit.Id);
            return null;
        }

        public static HashSet<Unit> GetFishUnitList(this UnitComponent self) =>
                                    self.GetTypeUnits(UnitType.Fish);

        public static void FixedUpdateFishUnitList(this UnitComponent self) =>
                           ForeachHelper.Foreach(self.GetFishUnitList(), BattleLogicUnit.FixedUpdate);

        public static Unit GetMaxScoreFishUnit(this UnitComponent self)
        {
            HashSet<Unit> fishUnitList = self.GetFishUnitList();
            if (fishUnitList.Count <= 0)
                return null;

            var battleLogicComponent = self.DomainScene().GetBattleLogicComponent();
            battleLogicComponent.Argument_Integer = 0;
            battleLogicComponent.Result_Unit = null;
            
            ForeachHelper.Foreach(fishUnitList, CompareFishScore);

            return battleLogicComponent.Result_Unit;
        }

        private static void CompareFishScore(this Unit fishUnit)
        {
            if (!fishUnit.FishUnitComponent.IsInScreen)
                return;

            var attributeComponent = fishUnit.GetComponent<NumericComponent>();
            int score = attributeComponent.GetAsInt(NumericType.Score);

            var battleLogicComponent = fishUnit.DomainScene().GetBattleLogicComponent();

            if (score <= battleLogicComponent.Argument_Integer)
                return;

            battleLogicComponent.Argument_Integer = score;
            battleLogicComponent.Result_Unit = fishUnit;
        }
    }
}