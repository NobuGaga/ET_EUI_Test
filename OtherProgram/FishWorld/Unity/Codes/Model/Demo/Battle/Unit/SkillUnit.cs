namespace ET
{
    /// <summary>
    /// 玩家技能 Unit, 不做成 Unit
    /// 简单的分为逻辑层跟视图层处理
    /// 同一个技能类型效果同一时间只会存在一种状态数据
    /// </summary>
    public class SkillUnit : Entity, IAwake<int, int>, IDestroy
    {
        /// <summary> 技能类型 </summary>
        public int SkillType => (int)Id;

        /// <summary> 技能效果结束时间戳 </summary>
        public long SkillEndTime;

        /// <summary> 技能冷却总时长(毫秒) </summary>
        public int CdRetainTime;

        /// <summary> 技能冷却结束时间戳 </summary>
        public long CdEndTime;

        public Entity GameObjectComponent;

        public Entity LaserSkillUnitComponent;
    }
}