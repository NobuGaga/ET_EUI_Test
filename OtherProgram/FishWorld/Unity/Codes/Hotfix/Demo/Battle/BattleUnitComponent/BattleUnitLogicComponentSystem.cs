// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
	[FriendClass(typeof(Unit))]
    public class FishUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            Unit unit = self.Awake(unitInfo);
            unit.FishUnitComponent = unit.AddComponent<FishUnitComponent>(BattleConfig.IsUseModelPool);
        }
    }

    [ObjectSystem]
	[FriendClass(typeof(Unit))]
	public class BulletUnitLogicComponentAwakeSystem : AwakeSystem<BattleUnitLogicComponent, UnitInfo,
                                                                   CannonShootInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(BattleUnitLogicComponent self, UnitInfo unitInfo,
                                   CannonShootInfo cannonShootInfo)
        {
            Unit unit = self.Awake(unitInfo);
            unit.BulletUnitComponent = unit.AddComponent<BulletUnitComponent, CannonShootInfo>
                                                        (cannonShootInfo, BattleConfig.IsUseModelPool);
        }
    }

    [FriendClass(typeof(Unit))]
    public static class BattleUnitLogicComponentSystem
    {
        internal static Unit Awake(this BattleUnitLogicComponent self, UnitInfo unitInfo)
        {
            Unit unit = self.Parent as Unit;

            unit.UnitId = unitInfo.UnitId;

            bool isUseModelPool = BattleConfig.IsUseModelPool;
            var attributeComponent = unit.AddComponent<NumericComponent>(isUseModelPool);

            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var attributeTypes = unitInfo.Ks;
            var attributeValues = unitInfo.Vs;

            if (unitInfo.Ks != null && unitInfo.Vs != null && unitInfo.Ks.Count == unitInfo.Vs.Count)
                for (var i = 0; i < attributeTypes.Count; i++)
                    attributeComponent.Set(attributeTypes[i], attributeValues[i]);

            unit.AttributeComponent = attributeComponent;
            unit.TransformComponent = unit.AddComponent<TransformComponent>(isUseModelPool);

            return unit;
        }

        public static void FixedUpdate(this Unit self)
        {
            if (self.Type == UnitType.Fish)
                self.FishUnitComponent.FixedUpdate(self);
            else if (self.Type == UnitType.Bullet)
                self.BulletUnitComponent.FixedUpdate(self);
        }
    }
}