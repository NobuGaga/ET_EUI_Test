namespace ET.Battle
{
    // 战斗单位, 用来储存战斗用数据的基础单位
    public class BattleUnit : Entity, IAwake<UnitInfo>, IDestroy
    {
        // 单位 ID (对应以前实体 ID (服务器), 避免混淆用 Unit 标识, 跟服务器保持一致)
        // 去除以前用于 C# <=> Lua 交互用的客户端实体 ID
        // 直接复用基类 Entity 的唯一标识字段 Id, 在 Factory.Create 进行 Id 的赋值
        public long UnitId => Id;
    }
}