using UnityEngine;

namespace ET
{
	/// <summary>
	/// 战斗单位视图组件, 用以拓展 Unit 单位, 作一些战斗用处理
	/// 用来储存战斗用视图相关引用
	/// </summary>
	public class BattleUnitViewComponent : Entity, IAwake<Scene, Unit>, IDestroy
	{
		public Transform NodeParent;
	}
}