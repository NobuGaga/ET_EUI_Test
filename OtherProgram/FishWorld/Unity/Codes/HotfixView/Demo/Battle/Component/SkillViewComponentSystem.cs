// Battle Review Before Boss Node

using System.Collections.Generic;

namespace ET
{
    public static class SkillViewComponentSystem
    {
        public static void UpdateBeforeBullet(this SkillComponent self)
        {
            HashSet<Unit> playerUnitList = self.GetPlayerUnitList();
            if (playerUnitList != null)
                ForeachHelper.Foreach(playerUnitList, UpdateBeforeBullet);
        }

        private static void UpdateBeforeBullet(this Unit playerUnit) =>
                            playerUnit.GetComponent<PlayerSkillComponent>().UpdateBeforeBullet();
    }
}