// Battle Review Before Boss Node

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    public static class FisheryHelper
    {
        public static Unit GetPlayerUnit(int seatId)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            UnitComponent unitComponent = battleLogicComponent.UnitComponent;
            
            var playerUnitList = unitComponent.GetPlayerUnitList();
            battleLogicComponent.Result_Unit = null;

            battleLogicComponent.Foreach(playerUnitList, SetFishUnitPauseState, seatId);

            return battleLogicComponent.Result_Unit;
        }

        private static bool SetFishUnitPauseState(Unit playerUnit, int seatId)
        {
            var attributeComponent = playerUnit.GetComponent<NumericComponent>();
            if (attributeComponent.GetAsInt(NumericType.Pos) != seatId)
                return true;

            var battleLogicComponent = BattleLogicComponent.Instance;
            battleLogicComponent.Result_Unit = playerUnit;
            return false;
        }
    }
}