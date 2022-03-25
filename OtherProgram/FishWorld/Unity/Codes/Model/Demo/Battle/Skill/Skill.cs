using UnityEngine;

namespace ET
{
    public class PlayerSkill: Entity, IAwake
    {
        public int SkillId;
        /// <summary>
        /// 技能持续时长 毫秒
        /// </summary>
        public int SkillTime;
        public int cdTime;

        /// <summary>
        /// 技能CD时长 毫秒
        /// </summary>
        public int CdTime
        {
            set
            {
                this.cdTime            = value;
                this.CdResumeTimeStamp = value + TimeInfo.Instance.ServerNow();
            }
            get
            {
                return this.cdTime;
            }
        }

        /// <summary>
        /// cd恢复的时间戳 毫秒
        /// </summary>
        public long CdResumeTimeStamp;
    }
}