using System;
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

		/// <summary> 上一次自动刷鱼时间戳(毫秒) </summary>
		public long LastCreateFishTime;

		/// <summary> 上一次自动刷鱼鱼组列表索引 </summary>
		public ushort AutoCreateFishGroupIndex;

		/// <summary> GM 协议消息结构体 </summary>
		public C2M_GM C2M_GM;

		public Action<string, UnityEngine.Object> Action_String_UnityObject;
	}
}