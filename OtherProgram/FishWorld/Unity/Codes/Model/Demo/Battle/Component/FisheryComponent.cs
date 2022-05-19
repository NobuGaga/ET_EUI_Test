// Battle Review Before Boss Node

namespace ET
{
	/// <summary> 渔场组件, 用来存储渔场的数据, 直接挂载到 Current Scene 上 </summary>
	public class FisheryComponent : Entity, IAwake, IDestroy
	{
		/// <summary>
		/// 房间类型, 客户端不需要房间的 ID, 只需要知道跟对应配置表相匹配的房间类型
		/// </summary>
		public int RoomType;

		/// <summary> 区域 ID, 用于读取配置设置摄像机 </summary>
		public int AreaId;

		/// <summary> 是否在播放主摄像机移动 Tween </summary>
		public bool IsMovingCamera;

		public System.Func<Unit, int, bool> FindFishUnitBySeatId;

		public System.Action<Unit> QuickMoveFish;
	}
}