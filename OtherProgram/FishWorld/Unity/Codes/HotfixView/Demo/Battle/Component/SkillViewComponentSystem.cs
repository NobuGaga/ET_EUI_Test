// Battle Review Before Boss Node

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(UnitComponent))]
    [FriendClass(typeof(SkillComponent))]
    [FriendClass(typeof(Unit))]
    public static class SkillViewHelper
    {
        public static void UpdateBeforeBullet()
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            var unitComponent = battleLogicComponent.UnitComponent;
            var skillComponent = battleLogicComponent.SkillComponent;
            var playerUnitList = unitComponent.GetPlayerUnitList();
            if (playerUnitList != null)
                ForeachHelper.Foreach(playerUnitList, skillComponent.UpdateBeforeBullet);
        }
    }
}