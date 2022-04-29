namespace ET
{
	/// <summary> GM 常用常量数据 </summary>
	public static class GMConfig
	{
		/// <summary> 自动刷鱼 ID 列表 </summary>
		public readonly static ushort[] FishBaseConfigIDList = new ushort[] { 8, 10, 16, 17 };

		/// <summary>
		/// 自动刷鱼间隔毫秒
		/// 毫秒 68 大概 600 条低面数鱼(102 - 350) (134 - 300)
		/// </summary>
		public const ushort CreateFishInterval = 8000;

		/// <summary> 刷鱼 GM 指令 </summary>
		public const string MakeFishGM = "MakeFish";
	}
}