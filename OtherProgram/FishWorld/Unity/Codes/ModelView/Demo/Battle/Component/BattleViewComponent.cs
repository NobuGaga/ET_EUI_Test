using System.Collections.Generic;

namespace ET
{
	/// <summary>
	/// 战斗视图组件, 用于控制热更层战斗视图逻辑, 具体实现在 BattleViewComponentSystem 文件中
	/// </summary>
	public class BattleViewComponent : Entity, IAwake, IUpdate, IDestroy
	{
		public static BattleViewComponent Instance;

		/// <summary> 当前帧实体化鱼 GameObject 数 </summary>
		public int CurrentInstantiateCount;

		/// <summary> 实例化鱼 GameObject Unit ID 队列 </summary>
		public Stack<long> InstantiateFishStack = new Stack<long>(ConstHelper.FisheryUnitCount);
	}
}