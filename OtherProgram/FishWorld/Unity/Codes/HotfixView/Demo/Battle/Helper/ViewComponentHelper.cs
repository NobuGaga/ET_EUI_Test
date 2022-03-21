namespace ET
{
    public static class ViewComponentHelper
    {
        public static ObjectPoolComponent GetObjectPoolComponent(Unit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Player:
                case UnitType.Cannon:
                    return unit.DomainScene().GetComponent<ObjectPoolComponent>();

                case UnitType.Bullet:
                case UnitType.Fish:
                    
                    BattleViewComponent battleViewComponent = null;
                    if (BattleTestConfig.IsAddBattleToZone)
                        battleViewComponent = unit.ZoneScene().GetComponent<BattleViewComponent>();
                    else if (BattleTestConfig.IsAddBattleToCurrent)
                        battleViewComponent = unit.DomainScene().GetComponent<BattleViewComponent>();
                    
                    return battleViewComponent.GetComponent<ObjectPoolComponent>();
            }

            return null;
        }
    }
}