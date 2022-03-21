namespace ET
{
    /// <summary> 每个 Unit 通用都有的数据引用或者组件都在这里设置 </summary>
    public static class UnitLogicSystem
    {
        public static BattleUnitLogicComponent BattleUnitLogicComponent(this Unit self)
                                        => self.GetComponent<BattleUnitLogicComponent>();
    }
}