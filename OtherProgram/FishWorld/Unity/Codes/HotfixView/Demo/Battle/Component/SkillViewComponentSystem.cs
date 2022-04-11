using System.Collections.Generic;

namespace ET
{
    public static class SkillViewComponentSystem
    {
        public static void UpdateBeforeBullet(this SkillComponent self)
        {
            HashSet<Unit> playerUnitList = self.GetPlayerUnitList();
            if (playerUnitList != null)
                foreach (Unit playerUnit in playerUnitList)
                    playerUnit.GetComponent<PlayerSkillComponent>().UpdateBeforeBullet();
        }
    }
}