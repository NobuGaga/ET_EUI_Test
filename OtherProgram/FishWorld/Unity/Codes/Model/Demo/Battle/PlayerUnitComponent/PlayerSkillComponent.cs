using System.Collections.Generic;

namespace ET
{
    /// <summary> Player Unit 技能管理器, 管理每个 Unit 下的技能数据 </summary>
    [ChildType(typeof(SkillUnit))]
    public class PlayerSkillComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 是否在自己自动发炮效果中 </summary>
        public bool IsAutoShoot;

        /// <summary> 
        /// 玩家当前技能锁定的鱼的 Unit ID
        /// 所有技能都保持同一个目标, 这样不会出现,
        /// 瞄准技能打一条鱼, 镭射打另外一条鱼
        /// 瞄准跟镭射可以同时使用
        /// </summary>
        public long TrackFishUnitId;

        public List<long> SkillTypeList = new List<long>();
    }
}