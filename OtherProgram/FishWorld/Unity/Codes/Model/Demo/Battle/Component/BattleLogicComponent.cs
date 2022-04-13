// Battle Review Before Boss Node

using System;
using System.Collections.Generic;

namespace ET
{
	/// <summary>
	/// 战斗逻辑组件, 用于控制热更层战斗逻辑
	/// 包括协议请求, 数据更新, 优化调用方法等
	/// 不同模块的功能实现方法会放到对应模块中去拓展 BattleLogicComponent
	/// 只需要通过任意 Scene 的 GetBattleLogicComponent() 接口就可以获取该组件
	/// </summary>
	public class BattleLogicComponent : Entity, IAwake, IDestroy
	{
        #region Network Class

        public C2M_Fire FireInfo;

		public C2M_Hit HitInfo;

		public C2M_SkillUse UseSkillInfo;

		#endregion

		#region Foreach Function

		/// <summary>
		/// 需要移除的 Unit ID List, Unit 容器使用 HashSet, 遍历使用 foreach
		/// 不能在遍历过程中进行增删, 使用另外一个列表记录, 再遍历列表进行删除
		/// </summary>
		public List<long> RemoveUnitIdList = new List<long>(2 ^ 9);

		public bool Argument_Bool;

		public int Argument_Integer;

		public Unit Result_Unit;

		public Action<Unit, bool> Action_Unit_Bool;

		public Action<Unit, int> Action_Unit_Integer;

		#endregion
	}
}