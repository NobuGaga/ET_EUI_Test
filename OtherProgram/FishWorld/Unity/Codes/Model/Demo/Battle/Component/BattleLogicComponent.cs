namespace ET
{
	/// <summary>
	/// 战斗逻辑组件, 用于控制热更层战斗数据逻辑, 具体实现在 BattleLogicComponentSystem 文件中
	/// </summary>
	public class BattleLogicComponent : Entity, IAwake, IDestroy//, Battle TODO IUpdate
	{

		public C2M_Fire FireInfo;

		public C2M_Hit HitInfo;
	}
}