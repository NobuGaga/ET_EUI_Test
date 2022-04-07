namespace ET
{
    [ObjectSystem]
    public class BulletUnitComponentAwakeSystem : AwakeSystem<BulletUnitComponent, CannonShootInfo>
    {
        public override void Awake(BulletUnitComponent self, CannonShootInfo cannonShootInfo)
        {
            Unit unit = self.Parent as Unit;

            BulletMoveInfo bulletMoveInfo = BulletMoveHelper.PopInfo();
            bulletMoveInfo.Reset();
            BulletMoveHelper.InitInfo(bulletMoveInfo, cannonShootInfo);

            CannonShootHelper.PushPool(cannonShootInfo);
            self.Info = bulletMoveInfo;
            self.InitTrackPosition();

            self.InitTransform();

            unit.GetComponent<BattleUnitLogicComponent>().IsUpdate = true;
        }
    } 

    [ObjectSystem]
    public class BulletUnitComponentDestroySystem : DestroySystem<BulletUnitComponent>
    {
        public override void Destroy(BulletUnitComponent self)
        {
            BulletMoveHelper.PushPool(self.Info);
            self.Info = null;
        }
    }

    public static class BulletUnitComponentSystem
    {
        public static void FixedUpdate(this BulletUnitComponent self)
        {
            BulletMoveInfo info = self.Info;
            self.UpdateTrackPosition();
            BulletMoveHelper.FixedUpdate(info);
            self.UpdateTransform();
        }

        public static void InitTransform(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            transformComponent.NodeName = self.GetNodeName();
            self.UpdateTransform();
        }

        private static void UpdateTransform(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            BulletMoveInfo info = self.Info;
            transformComponent.SetLocalPos(info.CurrentLocalPos);
            transformComponent.SetLocalRotation(info.CurrentRotation);
        }

        private static string GetNodeName(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            return string.Format(BulletConfig.NameFormat, unit.Id);
        }

        public static long GetTrackFishUnitId(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            var attributeComponent = unit.GetComponent<NumericComponent>();
            return attributeComponent[NumericType.TrackFishId];
        }

        public static void InitTrackPosition(this BulletUnitComponent self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();

            if (trackFishUnitId == BulletConfig.DefaultTrackFishUnitId)
            {
                self.Info.TrackPosition = BulletMoveDefaultInfo.TrackPosition;
                return;
            }

            var battleLogicComponent = self.DomainScene().GetBattleLogicComponent();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);
            TransformComponent transformComponent = fishUnit.GetComponent<TransformComponent>();
            self.Info.TrackPosition = transformComponent.ScreenPos;
        }

        /// <summary> 更新追踪鱼的屏幕坐标 </summary>
        private static void UpdateTrackPosition(this BulletUnitComponent self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();
            if (trackFishUnitId == BulletConfig.DefaultTrackFishUnitId)
                return;

            // 检查之前目标的合法性
            var battleLogicComponent = self.DomainScene().GetBattleLogicComponent();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);

            if (fishUnit == null || fishUnit.IsDisposed)
            {
                self.SetNormalBullet();
                return;
            }

            TransformComponent transformComponent = fishUnit.GetComponent<TransformComponent>();
            if (transformComponent.IsInScreen)
                self.Info.TrackPosition = transformComponent.ScreenPos;
            else
                self.SetNormalBullet();
        }

        private static void SetNormalBullet(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            var attributeComponent = unit.GetComponent<NumericComponent>();
            attributeComponent.Set(NumericType.TrackFishId, BulletConfig.DefaultTrackFishUnitId);
            self.Info.TrackPosition = BulletMoveDefaultInfo.TrackPosition;
        }
    }
}