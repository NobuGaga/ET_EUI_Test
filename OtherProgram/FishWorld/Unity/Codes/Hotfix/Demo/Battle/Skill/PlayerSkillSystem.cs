using UnityEngine;

namespace ET
{
    public static class PlayerSkillSystem
    {
        /// <summary>
        /// cd是否好了
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsCdReady(this PlayerSkill self)
        {
            long nowTime = TimeInfo.Instance.ServerNow();

            return nowTime >= self.CdResumeTimeStamp;
        }

        /// <summary>
        /// 技能效果是否结束
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool EffectIsEnd(this PlayerSkill self)
        {
            long nowTime = TimeInfo.Instance.ServerNow();

            return nowTime >= self.EffectEndTimeStamp;
        }
    }
}