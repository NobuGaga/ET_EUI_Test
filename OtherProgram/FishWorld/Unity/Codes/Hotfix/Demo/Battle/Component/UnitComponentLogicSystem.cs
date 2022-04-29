// Battle Review Before Boss Node

using System.Collections.Generic;

namespace ET
{
    /// <summary> 原 UnitComponent 组件数据逻辑拓展 </summary>
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(UnitComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
	public static class UnitComponentLogicSystem
    {
        public static HashSet<Unit> GetPlayerUnitList(this UnitComponent self) =>
                                    self.GetTypeUnits(UnitType.Player);

        internal static Unit AddFishUnit(this UnitComponent self, UnitInfo unitInfo)
        {
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            Unit unit = self.AddChildWithId<Unit, int, UnitInfo>(unitInfo.UnitId, unitInfo.ConfigId,
                                                                 unitInfo, isUseModelPool);

            // 处理完鱼的逻辑后, 判断数据合法性
            FishUnitComponent fishUnitComponent = unit.FishUnitComponent;
            if (fishUnitComponent.MoveInfo.IsMoveEnd)
            {
                self.RemoveChild(unit.Id);
                return null;
            }

            return unit;
        }

        public static HashSet<Unit> GetFishUnitList(this UnitComponent self) =>
                                    self.GetTypeUnits(UnitType.Fish);

        public static Unit GetMaxScoreFishUnit(this UnitComponent self)
        {
            HashSet<Unit> fishUnitList = self.GetFishUnitList();
            if (fishUnitList.Count <= 0)
                return null;

            var battleLogicComponent = BattleLogicComponent.Instance;
            battleLogicComponent.Argument_Integer = 0;
            battleLogicComponent.Result_Unit = null;
            
            ForeachHelper.Foreach(fishUnitList, self.CompareFishScore);

            return battleLogicComponent.Result_Unit;
        }

        public static void CompareFishScore(Unit fishUnit)
        {
            if (!fishUnit.FishUnitComponent.ScreenInfo.IsInScreen)
                return;

            var attributeComponent = fishUnit.GetComponent<NumericComponent>();
            int score = attributeComponent.GetAsInt(NumericType.Score);

            var battleLogicComponent = BattleLogicComponent.Instance;

            if (score <= battleLogicComponent.Argument_Integer)
                return;

            battleLogicComponent.Argument_Integer = score;
            battleLogicComponent.Result_Unit = fishUnit;
        }
    }
}