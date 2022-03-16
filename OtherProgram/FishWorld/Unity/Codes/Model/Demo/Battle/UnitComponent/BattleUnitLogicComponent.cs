namespace ET
{
	// 战斗单位组件, 用以拓展 Unit 单位, 作一些战斗用处理
	// 用来储存战斗用逻辑相关数据的基础单位
	public class BattleUnitLogicComponent : Entity, IAwake<UnitInfo>, ILateUpdate, IDestroy
	{
		// 单位 ID (对应以前实体 ID (服务器), 避免混淆用 Unit 标识, 跟服务器保持一致)
		// 去除以前用于 C# <=> Lua 交互用的客户端实体 ID
		// 直接复用基类 Entity 的唯一标识字段 Id, 在 Factory.Create 进行 Id 的赋值
		public long UnitId;
	}
}