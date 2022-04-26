using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public class PlayerSkillComponentAwakeSystem : AwakeSystem<PlayerSkillComponent>
    {
        public override void Awake(PlayerSkillComponent self)
        {
            self.IsAutoShoot = false;
            self.TrackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
        }
    }

    [ObjectSystem]
    public class PlayerSkillComponentDestroySystem: DestroySystem<PlayerSkillComponent>
    {
        public override void Destroy(PlayerSkillComponent self) =>
                             self.SkillTypeList.Clear();
    }

    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(SkillUnit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(PlayerSkillComponent))]
    public static class PlayerSkillLogicComponentSystem
    {
        public static void Set(this PlayerSkillComponent self, M2C_SkillUse message)
        {
            int skillType = message.SkillType;
            int skillTime = message.SkillTime;
            int skillCdTime = message.SkillCDTime;

            self.TrackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
            if (message.TargetId != null && message.TargetId.Count > 0)
                self.TrackFishUnitId = message.TargetId[0];

            SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);
            if (skillUnit != null)
            {
                skillUnit.Set(skillTime, skillCdTime);
                return;
            }

            bool isUseModelPool = BattleConfig.IsUseModelPool;
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
            if (skillUnit == null)
                Log.Error($"PlayerSkillComponent.Get skillType = { skillType }, is null");

            return skillUnit;
        }

        public static void FixedUpdateBeforeFish(this PlayerSkillComponent self)
        {
            List<int> skillTypeList = self.SkillTypeList;
            for (int index = skillTypeList.Count - 1; index >= 0 ; index--)
            {
                long skillType = skillTypeList[index];
                SkillUnit skillUnit = self.GetChild<SkillUnit>(skillType);

                // 触发一次修改技能结束时间戳当标识
                if (skillUnit.IsRunning() && TimeHelper.ServerNow() >= skillUnit.SkillEndTime)
                    // 技能效果时间到了之后情况时间戳, 保证事件只触发一次
                    // 后面逻辑通过时间戳是否大于零表示技能是否还在效果时间
                    skillUnit.SkillEndTime = 0;

                // 技能 CD 结束且技能不生效则移除, 技能 CD 时间戳不被修改
                if (skillUnit.IsCdEnd() && !skillUnit.IsRunning())
                {
                    skillTypeList.RemoveAt(index);
                    self.RemoveChild(skillType);
                }
            }

            self.UpdateMaxScoreFish();
        }

        private static void UpdateMaxScoreFish(this PlayerSkillComponent self)
        {
            ref long trackFishUnitId = ref self.TrackFishUnitId;
            if (!self.IsInSkill(SkillType.Aim) && !self.IsInSkill(SkillType.Laser))
            {
                trackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
                return;
            }

            UnitComponent unitComponent = self.DomainScene().GetComponent<UnitComponent>();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);

            if (fishUnit != null && !fishUnit.IsDisposed && fishUnit.FishUnitComponent.ScreenInfo.IsInScreen)
                return;

            fishUnit = unitComponent.GetMaxScoreFishUnit();
            trackFishUnitId = fishUnit != null ? fishUnit.Id : ConstHelper.DefaultTrackFishUnitId;
        }
    }
}