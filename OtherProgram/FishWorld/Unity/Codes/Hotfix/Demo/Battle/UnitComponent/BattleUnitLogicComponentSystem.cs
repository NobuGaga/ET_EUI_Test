namespace ET
{
    #region Life Circle

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
            self.IsUpdate = false;
            InitComponent(self, unitInfo);
        }

        /// <summary> 初始化组件, 根据 UnitType 类型增加对应组件 </summary>
        private void InitComponent(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            // 先添加服务器发送的属性值
            InitAttributeComponent(self, unitInfo);

            bool isUseModelPool = BattleTestConfig.IsUseModelPool;

            // 再添加变换组件
            self.AddComponent<TransformComponent>(isUseModelPool);

            switch (unitInfo.UnitType)
            {
                case UnitType.Fish:
                    self.AddComponent<FishMoveComponent, int>(unitInfo.ConfigId, isUseModelPool);
                    self.TransformComponent().NodeName = self.FishMoveComponent().GetNodeName();
                    break;
            }
        }

        // 在 Unit 节点下 Add, 保持跟其他 Unit 一致
        private void InitAttributeComponent(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            var attributeComponent = self.Unit().AddComponent<NumericComponent>(isUseModelPool);

            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var attributeTypes = unitInfo.Ks;
            var attributeValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;

            for (ushort i = 0; i < attributeTypes.Count; i++)
                attributeComponent.Set(attributeTypes[i], attributeValues[i]);
        }
    }

	[ObjectSystem]
	public class BattleUnitLogicComponentDestroySystem : DestroySystem<BattleUnitLogicComponent>
	{
		public override void Destroy(BattleUnitLogicComponent self)
		{
            // Battle TODO
        }
	}

    #endregion

    #region Component Getter

    /// <summary>
    /// BattleUnitComponent 添加的子组件都在这里实现获取方法
    /// 其子组件也只能通过 BattleUnitComponent() 获取交互
    /// 子组件间的获取也相应在这里定义方法
    /// </summary>
    public static class BattleUnitLogicComponentChildComponentSystem
    {
        public static Unit Unit(this BattleUnitLogicComponent self) => self.Parent as Unit;

        public static NumericComponent AttributeComponent(this BattleUnitLogicComponent self)
                                                    => self.Unit().GetComponent<NumericComponent>();

        public static TransformComponent TransformComponent(this BattleUnitLogicComponent self)
                                                    => self.GetComponent<TransformComponent>();

        public static FishMoveComponent FishMoveComponent(this BattleUnitLogicComponent self)
                                                    => self.GetComponent<FishMoveComponent>();                                              
    }

    #endregion

    #region Base Function

    public static class BattleUnitLogicComponentSystem
    {
        public static void FixedUpdate(this BattleUnitLogicComponent self) =>
                                                            self.FishMoveComponent().FixedUpdate();
    }

    #endregion
}