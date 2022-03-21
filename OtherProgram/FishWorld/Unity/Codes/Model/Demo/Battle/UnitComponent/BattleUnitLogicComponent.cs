namespace ET
{
	// 战斗单位逻辑组件, 用以拓展 Unit 单位, 作一些战斗用处理
	// 用来储存战斗用逻辑相关数据
	public class BattleUnitLogicComponent : Entity, IAwake<UnitInfo>, IDestroy
	{
		/// <summary>
		/// 单位 ID (对应以前实体 ID (服务器), 避免混淆用 Unit 标识, 跟服务器保持一致)
		/// 去除以前用于 C# <=> Lua 交互用的客户端实体 ID
		/// </summary>
		public long UnitId;

		/// <summary>
		/// Battle TODO
		/// 更新标识, 旧框架在开始移动时设置 true, remove 时设置 false
		/// 设置在这里是因为默认战斗单位都需要更新操作
		/// </summary>
		public bool IsUpdate;
	}
}