namespace ET
{
	/// <summary>
	/// 战斗单位逻辑组件, 用以拓展 Unit 单位, 作一些战斗用处理
	/// 用来储存战斗用逻辑相关数据
	/// 在创建 Unit 的时候添加该组件
	/// </summary>
	public class BattleUnitLogicComponent : Entity, IAwake<UnitInfo>, IAwake<UnitInfo, CannonShootInfo>
	{

	}
}