using System.Collections.Generic;

namespace ET
{
    /// <summary> Player Unit 技能管理器, 管理每个 Unit 下的技能数据 </summary>
    public class PlayerSkillComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 是否在自己自动发炮效果中 </summary>
        public bool IsAutoShoot;

        public List<long> SkillTypeList = new List<long>();
    }
}