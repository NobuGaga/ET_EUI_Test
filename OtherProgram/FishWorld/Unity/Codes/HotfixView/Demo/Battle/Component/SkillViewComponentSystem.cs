// Battle Review Before Boss Node

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(UnitComponent))]
    [FriendClass(typeof(Unit))]
    public static class SkillViewComponentSystem
    {
        public static void UpdateBeforeBullet(this SkillComponent self)
        {
            var playerUnitList = BattleLogicComponent.Instance.UnitComponent.GetPlayerUnitList();
            if (playerUnitList != null)
                ForeachHelper.Foreach(playerUnitList, UpdateBeforeBullet);
        }

        private static void UpdateBeforeBullet(this Unit playerUnit) =>
                            playerUnit.PlayerSkillComponent.UpdateBeforeBullet();
    }
}