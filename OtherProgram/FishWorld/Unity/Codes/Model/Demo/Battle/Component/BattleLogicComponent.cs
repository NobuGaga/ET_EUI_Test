// Battle Review Before Boss Node

using System;

namespace ET
{
	/// <summary>
	/// 战斗逻辑组件, 用于控制热更层战斗逻辑
	/// 包括协议请求, 数据更新, 优化调用方法等
	/// 不同模块的功能实现方法会放到对应模块中去拓展 BattleLogicComponent
	/// </summary>
	public class BattleLogicComponent : Entity, IAwake, IDestroy
	{
		public static BattleLogicComponent Instance;

		public Scene CurrentScene;

		public Scene ZoneScene;

		#region Reference Component

		public UnitComponent UnitComponent;

		public BulletLogicComponent BulletLogicComponent;

		public SkillComponent SkillComponent;

		public FisheryComponent FisheryComponent;

		#endregion

		#region Network

		public SessionComponent SessionComponent;

		public C2M_Fire FireInfo;

		public C2M_Hit HitInfo;

		public C2M_SkillUse UseSkillInfo;

		#endregion

		#region Foreach Function

		public bool Argument_Bool;

		public int Argument_Integer;

		public string Argument_String;

		public Unit Result_Unit;

		public Action<Unit, bool> Action_Unit_Bool;

		public Func<Unit, int, bool> BreakFunc_Unit_Integer;

		#endregion
	}
}