// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
	[FriendClass(typeof(Unit))]
    public class FishUnitLogicComponentAwakeSystem : AwakeSystem<Unit, int, UnitInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(Unit self, int configId, UnitInfo unitInfo)
        {
            UnitMonoComponent.Instance.AddFishUnit(unitInfo.UnitId);

            self.Awake(unitInfo);

            self.ConfigId = configId;
            self.FishUnitComponent = self.AddComponent<FishUnitComponent>(BattleConfig.IsUseModelPool);
        }
    }

    [ObjectSystem]
	[FriendClass(typeof(Unit))]
	public class BulletUnitLogicComponentAwakeSystem : AwakeSystem<Unit, UnitInfo,
                                                                   CannonShootInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        public override void Awake(Unit self, UnitInfo unitInfo, CannonShootInfo cannonShootInfo)
        {
            UnitMonoComponent.Instance.AddBulletUnit(unitInfo.UnitId);

            self.Awake(unitInfo);

            self.ConfigId = BulletConfig.BulletColliderID;
            self.BulletUnitComponent = self.AddComponent<BulletUnitComponent, CannonShootInfo>
                                                        (cannonShootInfo, BattleConfig.IsUseModelPool);
        }
    }

    [FriendClass(typeof(Unit))]
    public static class BattleUnitLogicComponentSystem
    {
        internal static void Awake(this Unit self, UnitInfo unitInfo)
        {
            self.UnitId = unitInfo.UnitId;

            self.UnitType = unitInfo.Type;
            self.Type = unitInfo.Type;

            bool isUseModelPool = BattleConfig.IsUseModelPool;
            var attributeComponent = self.AddComponent<NumericComponent>(isUseModelPool);

            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var attributeTypes = unitInfo.Ks;
            var attributeValues = unitInfo.Vs;

            if (unitInfo.Ks != null && unitInfo.Vs != null && unitInfo.Ks.Count == unitInfo.Vs.Count)
                for (var i = 0; i < attributeTypes.Count; i++)
                    attributeComponent.SetNoEvent(attributeTypes[i], attributeValues[i]);

            self.AttributeComponent = attributeComponent;
            self.TransformComponent = self.AddComponent<TransformComponent>(isUseModelPool);
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