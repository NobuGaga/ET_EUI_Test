using UnityEngine;

namespace ET
{
    public class PlayerSkill: Entity, IAwake
    {
        public int SkillId;
        public int skillTime;

        /// <summary>
        /// 技能持续时长 毫秒
        /// </summary>
        public int SkillTime
        {
            get
            {
                return this.skillTime;
            }
            set
            {
                this.skillTime          = value;
                this.EffectEndTimeStamp = value + TimeInfo.Instance.ServerNow();
            }
        }

        /// <summary>
        /// 技能效果结束的时间戳
        /// </summary>
        public long EffectEndTimeStamp;

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

        /// <summary>
        /// 是否在技能生效期间
        /// </summary>
        public bool IsRunning;
    }
}