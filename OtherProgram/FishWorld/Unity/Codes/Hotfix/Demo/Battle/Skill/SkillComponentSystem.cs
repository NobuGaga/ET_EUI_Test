using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET
{
    [Timer(TimerType.SkillTimer)]
    public class SkillTimer: ATimer<SkillComponent>
    {
        public override void Run(SkillComponent self)
        {
            try
            {
                self.UpdateSkill();
            }
            catch (Exception e)
            {
                Log.Error($"skill timer error: {self.Id}\n{e}");
            }
        }
    }

    [ObjectSystem]
    public class SkillComponentAwakeSystem: AwakeSystem<SkillComponent>
    {
        public override void Awake(SkillComponent self)
        {
            self.Timer = TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.SkillTimer, self);
        }
    }

    [ObjectSystem]
    public class SkillComponentDestroySystem: DestroySystem<SkillComponent>
    {
        public override void Destroy(SkillComponent self)
        {
            TimerComponent.Instance?.Remove(ref self.Timer);
        }
    }

    public static class SkillComponentSystem
    {
        public static void UpdateSkill(this SkillComponent self)
        {
            for (int i = 0; i < self.SkillIds.Count; i++)
            {
                int         skillId     = self.SkillIds[i];
                PlayerSkill playerSkill = self.GetSkill(skillId);

                if (!playerSkill.IsRunning)
                {
                    continue;
                }

                if (!playerSkill.EffectIsEnd())
                {
                    continue;
                }

                playerSkill.IsRunning = false;

                Game.EventSystem
                    .PublishAsync(new EventType.SkillEnd { CurrentScene = self.DomainScene(), UnitId = self.Parent.Id, SkillId = skillId })
                    .Coroutine();
            }
        }

        public static async ETTask SetSkill(this SkillComponent self, M2C_SkillUse message)
        {
            PlayerSkill playerSkill = self.GetSkill(message.SkillId);
            bool        isSend      = !playerSkill.IsRunning;

            playerSkill.CdTime    = message.SkillCDTime;
            playerSkill.SkillTime = message.SkillTime;
            playerSkill.IsRunning = true;

            // 技能效果持续时间不重复发送
            if (isSend)
            {
                await Game.EventSystem.PublishAsync(new EventType.SkillUse { CurrentScene = self.DomainScene(), Message = message });
            }
        }

        public static PlayerSkill GetSkill(this SkillComponent self, int skillId)
        {
            PlayerSkill playerSkill = self.GetChild<PlayerSkill>(skillId);

            if (null == playerSkill)
            {
                playerSkill = self.AddChildWithId<PlayerSkill>(skillId);

                playerSkill.SkillId = skillId;

                self.SkillIds.Add(skillId);
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