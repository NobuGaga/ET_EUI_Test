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
            self.Info = bulletMoveInfo;

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
    }
}