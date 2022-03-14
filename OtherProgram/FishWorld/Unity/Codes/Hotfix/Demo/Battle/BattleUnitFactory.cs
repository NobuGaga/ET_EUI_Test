namespace ET.Battle
{
    public static class BattleUnitFactory
    {
        public static void Create(UnitInfo unitInfo)
        {
            // 这里的创建不使用 currentScene 是因为 BattleLogicComponent 可能加到 zoneScene 里
            BattleUnit battleUnit = BattleLogicComponent.Instance.CreateBattleUnit(unitInfo);
            Game.EventSystem.Publish(new BattleEventType.AfterBattleUnitCreate()
            {
                BattleUnit = battleUnit
            });
        }
    }
}