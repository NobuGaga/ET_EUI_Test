namespace ET
{
    #region Life Circle

    /// <summary>
    /// 原来自带代码实现了 IAwake<int> 用来复制表 Id
    /// 因为并不是每个战斗实体都有表 Id 所以这里不作赋值跟保存
    /// 转到具体的鱼组件去赋值
    /// 现通过增加组件的方式拓展
    /// 在创建 Unit 的时候添加该组件
    /// 用来实现跟管理战斗的 Unit 行为跟数据储存
    /// </summary>
    [ObjectSystem]
	public class BattleUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo, CannonShootInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        // 在 UnitFactory 进行类型判断对应的使用 AddChild 传入参数
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo, CannonShootInfo cannonShootInfo)
        {
            Unit unit = self.Parent as Unit;

            InitAttributeComponent(unit, unitInfo);
            unit.AddComponent<TransformComponent>(BattleTestConfig.IsUseModelPool);
            unit.AddComponent<ColliderLogicComponent>(BattleTestConfig.IsUseModelPool);

            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    unit.AddComponent<FishUnitComponent>(BattleTestConfig.IsUseModelPool);
                    break;
                case UnitType.Bullet:
                    unit.AddComponent<BulletUnitComponent, CannonShootInfo>(cannonShootInfo, BattleTestConfig.IsUseModelPool);
                    break;
            }
        }

        private void InitAttributeComponent(Unit unit, UnitInfo unitInfo)
        {
            var attributeCom = unit.AddComponent<NumericComponent>(BattleTestConfig.IsUseModelPool);

            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var attributeTypes = unitInfo.Ks;
            var attributeValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;

            for (ushort i = 0; i < attributeTypes.Count; i++)
                attributeCom.Set(attributeTypes[i], attributeValues[i]);
        }
    }

	[ObjectSystem]
	public class BattleUnitLogicComponentDestroySystem : DestroySystem<BattleUnitLogicComponent>
	{
		public override void Destroy(BattleUnitLogicComponent self) => self.IsUpdate = false;
	}

    #endregion

    #region Base Function

    public static class BattleUnitLogicComponentSystem
    {
        public static void FixedUpdate(this Unit self)
        {
            switch (self.UnitType)
            {
                case UnitType.Fish:
                    self.GetComponent<FishUnitComponent>().FixedUpdate();
                    return;
                case UnitType.Bullet:
                    self.GetComponent<BulletUnitComponent>().FixedUpdate();
                    return;
            }
        }
    }

    #endregion
}