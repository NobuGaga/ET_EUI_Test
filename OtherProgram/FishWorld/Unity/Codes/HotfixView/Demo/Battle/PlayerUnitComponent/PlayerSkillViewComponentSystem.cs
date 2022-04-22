using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(PlayerSkillComponent))]
    public static class PlayerSkillViewComponentSystem
    {
        public static void UpdateBeforeBullet(this PlayerSkillComponent self)
        {
            List<long> skillTypeList = self.SkillTypeList;
            for (ushort index = 0; index < skillTypeList.Count; index++)
            {
                SkillUnit skillUnit = self.Get((int)skillTypeList[index]);
                skillUnit.UpdateBeforeBullet();
            }
        }
    }
}