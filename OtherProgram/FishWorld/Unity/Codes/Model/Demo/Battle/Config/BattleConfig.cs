namespace ET
{
	/// <summary> 战斗测试参数代码配置数据 </summary>
	public static class BattleConfig
	{
		/// <summary> 是否在创建 Battle Unit 跟 Component 的时候使用池 </summary>
		public static bool IsUseModelPool = true;

		/// <summary> 每帧最多实例化 GameObject 数 </summary>
		public const int FrameInstantiateObjectCount = 5;

		/// <summary> 是否启动自动刷鱼 </summary>
		public static bool IsAutoCreateFish = false;
	}
}