// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    public class FishUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            self.Awake(unitInfo);
            self.Parent.AddComponent<FishUnitComponent>(BattleConfig.IsUseModelPool);
        }
    }

    [ObjectSystem]
	public class BulletUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo, CannonShootInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo, CannonShootInfo cannonShootInfo)
        {
            self.Awake(unitInfo);
            self.Parent.AddComponent<BulletUnitComponent, CannonShootInfo>(cannonShootInfo, BattleConfig.IsUseModelPool);
        }
    }

    public static class BattleUnitLogicComponentSystem
    {
        internal static void Awake(this BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            Unit unit = self.Parent as Unit;
            unit.InitAttributeComponent(unitInfo);
            unit.AddComponent<TransformComponent>(BattleConfig.IsUseModelPool);
            unit.AddComponent<ColliderLogicComponent>(BattleConfig.IsUseModelPool);
        }

        private static void InitAttributeComponent(this Unit unit, UnitInfo unitInfo)
        {
            var attributeCom = unit.AddComponent<NumericComponent>(BattleConfig.IsUseModelPool);

            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var attributeTypes = unitInfo.Ks;
            var attributeValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;

            for (var i = 0; i < attributeTypes.Count; i++)
                attributeCom.Set(attributeTypes[i], attributeValues[i]);
        }

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
}