namespace ET
{
    public static class ViewComponentHelper
    {
        public static ObjectPoolComponent GetObjectPoolComponent(Unit unit)
        {
            UnitType unitType = unit.UnitType;
            if (unitType == UnitType.Player || unitType == UnitType.Player)
                return unit.DomainScene().GetComponent<ObjectPoolComponent>();

            Scene scene = BattleTestConfig.IsAddBattleToZone ? unit.ZoneScene() : unit.DomainScene();
            BattleViewComponent battleViewCom = scene.GetComponent<BattleViewComponent>();
            return battleViewCom.GetComponent<ObjectPoolComponent>();
        }
    }
}