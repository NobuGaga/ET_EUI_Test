using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public class PlayerSkillComponentAwakeSystem : AwakeSystem<PlayerSkillComponent>
    {
        public override void Awake(PlayerSkillComponent self)
        {
            // Battle TODO
        }
    }

    [ObjectSystem]
    public class PlayerSkillComponentDestroySystem: DestroySystem<PlayerSkillComponent>
    {
        public override void Destroy(PlayerSkillComponent self) =>
                             self.SkillTypeList.Clear();
    }

    public static class PlayerSkillComponentSystem
    {
        public static void Set(this PlayerSkillComponent self, int skillType, int skillTime, int skillCdTime)
        {
            SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);
            if (skillUnit != null)
            {
                skillUnit.Set(skillTime, skillCdTime);
                return;
            }

            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            skillUnit = self.AddChildWithId<SkillUnit, int, int>(skillType, skillTime, skillCdTime, isUseModelPool);
            self.SkillTypeList.Add(skillType);

            Game.EventSystem.Publish(new AfterCreateSkillUnit
            {
                CurrentScene = self.DomainScene(),
                SkillUnit = skillUnit,
            });
        }

        /// <summary> 是否用过技能, 技能还在冷却或者是技能效果还在则返回 true </summary>
        public static bool IsUsedSkill(this PlayerSkillComponent self, int skillType)
        {
            SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);
            return skillUnit != null && !skillUnit.IsDisposed;
        }

        public static bool IsInSkill(this PlayerSkillComponent self, int skillType)
        {
            if (!self.IsUsedSkill(skillType))
                return false;

            SkillUnit skillLogicUnit = self.Get(skillType);
            return skillLogicUnit.IsInSkill();
        }

        /// <summary> Get 之前判断一下 IsUsedSkill </summary>
        public static SkillUnit Get(this PlayerSkillComponent self, int skillType)
        {
            SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);
            if (skillUnit != null)
                return skillUnit;

            Log.Error($"PlayerSkillComponent.Get skillType = { skillType }, is null");
            return null;
        }

        public static void FixedUpdate(this PlayerSkillComponent self)
        {
            Unit playerUnit = self.Parent as Unit;
            List<long> skillTypeList = self.SkillTypeList;
            for (int index = skillTypeList.Count - 1; index >= 0 ; index--)
            {
                long skillType = skillTypeList[index];
                SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);

                // 触发一次修改技能结束时间戳当标识
                if (skillUnit.IsSkillEnd())
                    skillUnit.SkillEnd(playerUnit.Id);

                if (skillUnit.IsInSkill())
                    skillUnit.FixedUpdate();

                // 技能 CD 结束且技能不生效则移除, 技能 CD 时间戳不被修改
                if (skillUnit.IsCdEnd() && !skillUnit.IsRunning())
                {
                    skillTypeList.RemoveAt(index);
                    self.RemoveChild(skillType);
                }
            }
        }
    }
}