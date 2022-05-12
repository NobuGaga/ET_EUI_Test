// Battle Review Before Boss Node

using System.Collections.Generic;

namespace ET
{
	/// <summary> 子弹逻辑组件 </summary>
	[ChildType(typeof(Unit))]
	public class BulletLogicComponent : Entity, IAwake, IDestroy
	{
		/// <summary> 客户端生成的子弹 ID 计数器 </summary>
		public long BulletId;

		/// <summary> 上一次发射子弹时间戳(毫秒) </summary>
		public long LastShootBulletTime;

		/// <summary> 自己发射的子弹个数 </summary>
		public ushort ShootBulletCount;

		/// <summary> 用于客户端本地创建 Unit 统一使用的 UnitInfo </summary>
		public UnitInfo UnitInfo;

		/// <summary>
		/// 客户端生成单次使用的子弹 ID 计数器
		/// 只增不减, 退出渔场时清零
		/// 通过复用缓存值保证唯一性
		/// </summary>
		public long OneHitBulletId;

		public Stack<long> OneHitBulletIdStack;
	}
}