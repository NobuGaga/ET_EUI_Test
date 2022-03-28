using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public static class SkillHelper
    {
        public static void UseSkill(Scene zoneScene, int skillId, List<long> targetIdList)
        {
            zoneScene.GetComponent<SessionComponent>().Session.Send(new C2M_SkillUse() { SkillId = skillId, TargetId = targetIdList });
        }
    }
}