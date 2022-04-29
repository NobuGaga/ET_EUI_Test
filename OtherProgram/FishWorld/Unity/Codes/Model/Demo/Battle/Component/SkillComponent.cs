using System;

namespace ET
{
	/// <summary> 挂载在场景的技能组件, 用来存储渔场的技能数据 </summary>
	public class SkillComponent : Entity, IAwake, IDestroy
	{
		/// <summary>
		/// Battle TODO 冰冻技能结束时间戳(毫秒), 后面看看使用 kv 形式去拓展技能状态等
		/// </summary>
		public long IceEndTime;

		public Action<Unit> FixedUpdateBeforeFish;

		public Action<Unit> UpdateBeforeBullet;

		public Action<Unit> SetFishAnimatorState;
	}
}