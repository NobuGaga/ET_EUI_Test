namespace ET
{
	/// <summary>
	/// 渔场组件, 用来存储渔场的数据, 直接挂载到 Current Scene 上
	/// </summary>
	public class FisheryComponent : Entity, IAwake
	{
		/// <summary>
		/// 房间类型, 客户端不需要房间的 ID, 只需要知道跟对应配置表相匹配的房间类型
		/// </summary>
		public int RoomType;

		/// <summary>
		/// Battle TODO 冰冻技能剩余时间(毫秒), 后面看看使用 kv 形式去拓展技能状态等
		/// </summary>
		public int LeftIceTime;

		/// <summary>
		/// 区域 ID, 用于读取配置设置摄像机
		/// </summary>
		public int AreaId;
	}
}