namespace ET
{
	/// <summary> 战斗测试参数代码配置数据 </summary>
	public static class BattleConfig
	{
		/// <summary> 是否将战斗模块加到 ZoneScene 下面 </summary>
		public static bool IsAddToZoneScene = false;

		/// <summary> 是否将战斗模块加到 CurrentScene 下面 </summary>
		public static bool IsAddToCurrentScene = !IsAddToZoneScene;

		/// <summary> 是否在创建 Battle Unit 跟 Component 的时候使用池 </summary>
		public const bool IsUseModelPool = false;
	}
}