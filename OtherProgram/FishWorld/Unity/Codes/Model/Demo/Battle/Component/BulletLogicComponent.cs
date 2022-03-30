using System.Collections.Generic;

namespace ET
{
	/// <summary>
	/// 子弹逻辑组件
	/// </summary>
	public class BulletLogicComponent : Entity, IAwake, IDestroy//, Battle TODO IUpdate
	{
		/// <summary> 客户端生成的子弹 ID 计数器 </summary>
		public long BulletId;

		/// <summary> 自己发射的子弹个数 </summary>
		public ushort ShootBulletCount;

		/// <summary> 用于客户端本地创建 Unit 统一使用的 UnitInfo </summary>
		public UnitInfo unitInfo = new UnitInfo();

		/// <summary> 渔场内存在的子弹 ID 列表, 加快遍历速度 </summary>
		public List<long> BulletIdList = new List<long>(FisheryConfig.FisheryMaxBulletCount);
	}
}