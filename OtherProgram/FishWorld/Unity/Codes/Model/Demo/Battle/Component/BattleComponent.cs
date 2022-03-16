// 战斗模块命名空间
namespace ET
{
	/// <summary>
	/// 战斗逻辑组件, 用于控制热更层战斗数据逻辑, 具体实现在 BattleLogicComponentSystem 文件中
	/// </summary>
	public class BattleLogicComponent : Entity, IAwake
	{
		public static BattleLogicComponent Instance;

		public BattleLogicComponent() => Instance = this;

		~BattleLogicComponent()
		{
			Instance = null;
		}
	}

	/// <summary>
	/// 战斗视图组件, 用于控制热更层战斗视图逻辑, 具体实现在 BattleViewComponentSystem 文件中
	/// </summary>
	public class BattleViewComponent : Entity, IAwake
	{
		public static BattleViewComponent Instance;

		public BattleViewComponent() => Instance = this;

		~BattleViewComponent()
		{
			Instance = null;
		}
	}
}