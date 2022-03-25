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

		/// <summary> 渔场内存在的子弹 ID 列表, 加快遍历速度 </summary>
		public List<long> BulletIdList = new List<long>(FisheryConfig.FisheryMaxBulletCount);

		/// <summary>
		/// 储存渔场内子弹 Unit 的字典, 因为子弹的 Unit ID 是客户端生成的
		/// 为避免跟 UnitComponent 里的 Unit ID 重复导致查询失败
		/// 放在战斗管理来储存子弹 Unit 数据
		/// </summary>
		public Dictionary<long, Unit> BulletUnitMap = new Dictionary<long, Unit>(FisheryConfig.FisheryMaxBulletCount);
	}
}