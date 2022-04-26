namespace ET
{
    [ObjectSystem]
    public class SkillUnitAwakeSystem : AwakeSystem<SkillUnit, int, int>
    {
        public override void Awake(SkillUnit self, int skillTime, int skillCdTime) =>
                             self.Set(skillTime, skillCdTime);
    }

    [ObjectSystem]
    public class SkillUnitDestroySystem : DestroySystem<SkillUnit>
    {
        public override void Destroy(SkillUnit self)
        {
            // Battle TODO
        }
    }

    [FriendClass(typeof(SkillUnit))]
    public static class SkillUnitLogicSystem
    {
        /// <summary> 技能效果是否存在 </summary>
        public static bool IsRunning(this SkillUnit self) => self.SkillEndTime > 0;

        /// <summary> 技能效果是否结束 </summary>
        public static bool IsInSkill(this SkillUnit self) =>
                           self.IsRunning() && TimeHelper.ServerNow() < self.SkillEndTime;

        /// <summary> 技能 CD 是否结束 </summary>
        public static bool IsCdEnd(this SkillUnit self) =>
                           TimeHelper.ServerNow() >= self.CdEndTime;

        public static void Set(this SkillUnit self, int skillTime, int skillCdTime)
        {
            long nowServerTime = TimeHelper.ServerNow();
            self.SkillEndTime = skillTime + nowServerTime;
            self.CdRetainTime = skillCdTime;
            self.CdEndTime = skillCdTime + nowServerTime;
        }
    }
}