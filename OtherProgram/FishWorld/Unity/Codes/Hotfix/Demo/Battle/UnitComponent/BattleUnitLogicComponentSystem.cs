namespace ET
{
    /// <summary>
    /// 原来自带代码实现了 IAwake<int> 用来复制表 Id
    /// 因为并不是每个战斗实体都有表 Id 所以这里不作赋值跟保存
    /// 转到具体的鱼组件去赋值
    /// 现通过增加组件的方式拓展
    /// 在创建 Unit 的时候添加该组件
    /// </summary>
    [ObjectSystem]
	public class BattleUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo>
	{
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        // 在 UnitFactory 进行类型判断对应的使用 AddChild 传入参数
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            self.UnitId = unitInfo.UnitId;
            InitAttrData(self, unitInfo);
            self.AddComponent<TransformComponent>();
        }

        private void InitAttrData(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            NumericComponent numericComponent = self.AddComponent<NumericComponent>();
            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var numericTypes = unitInfo.Ks;
            var numericValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;

            for (int i = 0; i < numericTypes.Count; i++)
                numericComponent.Set(numericTypes[i], numericValues[i]);
        }
    }
	
	[ObjectSystem]
	public class BattleUnitLogicComponentDestroySystem : DestroySystem<BattleUnitLogicComponent>
	{
		public override void Destroy(BattleUnitLogicComponent self)
		{

		}
	}

    /// <summary>
    /// BattleUnitComponent 添加的子组件都在这里实现获取方法
    /// 其子组件也只能通过 BattleUnitComponent() 获取交互
    /// 子组件间的获取也相应在这里定义方法
    /// </summary>
    public static class BattleUnitLogicComponentChildComponentSystem
    {
        public static BattleUnitLogicComponent BattleUnitComponent(this TransformComponent self)
                                                        => self.Parent as BattleUnitLogicComponent;

        public static TransformComponent TransformComponent(this BattleUnitLogicComponent self)
                                                    => self.GetComponent<TransformComponent>();
    }

    public static class BattleUnitLogicComponentSystem
    {
        public static Unit Unit(this BattleUnitLogicComponent self) => self.Parent as Unit;
    }
}