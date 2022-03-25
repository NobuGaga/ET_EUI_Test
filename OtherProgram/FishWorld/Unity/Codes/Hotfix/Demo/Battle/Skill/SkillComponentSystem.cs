using System.Collections.Generic;
using ET.Battle;
using UnityEngine;

namespace ET
{
    public static class SkillComponentSystem
    {
        public static void SetSkill(this SkillComponent self, M2C_SkillUse message)
        {
            PlayerSkill playerSkill = self.GetSkill(message.SkillId);

            playerSkill.CdTime  = message.SkillCDTime;
            playerSkill.SkillTime  = message.SkillTime;
        }

        public static PlayerSkill GetSkill(this SkillComponent self, int skillId)
        {
            PlayerSkill playerSkill = self.GetChild<PlayerSkill>(skillId);

            if (null == playerSkill)
            {
                playerSkill = self.AddChildWithId<PlayerSkill>(skillId);

                playerSkill.SkillId = skillId;
            }

            return playerSkill;
        }

        public static void UseSkill(this SkillComponent self, int skillId)
        {
            PlayerSkill playerSkill = self.GetSkill(skillId);

            if (!playerSkill.IsCdReady())
            {
                return;
            }
            
            Scene zoneScene = self.ZoneScene();
            
            SkillHelper.UseSkill(zoneScene, skillId, new List<long>());
        }
    }
}