using UnityEngine;

namespace ET
{
	// 战斗单位组件, 用以拓展 Unit 单位, 作一些战斗用处理
	// 用来储存战斗用视图相关数据的基础单位
	public class BattleUnitViewComponent : Entity, IAwake, IDestroy
	{
		public Transform NodeParent;
	}
}