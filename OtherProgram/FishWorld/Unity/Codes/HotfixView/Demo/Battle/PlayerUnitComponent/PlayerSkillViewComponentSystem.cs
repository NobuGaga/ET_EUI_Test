using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(PlayerSkillComponent))]
    public static class PlayerSkillViewComponentSystem
    {
        public static void UpdateBeforeBullet(this PlayerSkillComponent self)
        {
            List<int> skillTypeList = self.SkillTypeList;
            for (ushort index = 0; index < skillTypeList.Count; index++)
                self.Get(skillTypeList[index])?.UpdateBeforeBullet();
        }
    }
}