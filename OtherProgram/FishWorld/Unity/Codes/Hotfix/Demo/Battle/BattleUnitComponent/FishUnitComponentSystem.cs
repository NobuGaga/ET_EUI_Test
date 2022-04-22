namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    public class FishUnitComponentAwakeSystem : AwakeSystem<FishUnitComponent>
    {
        public override void Awake(FishUnitComponent self)
        {
            Unit unit = self.Parent as Unit;

            FishMoveInfo info = FishMoveHelper.PopInfo();
            info.Reset();

            // 这里用 var 看起来不像 NumericComponent 组件 = =
            var attributeComponent = unit.AttributeComponent;
            // 鱼线表 ID
            short roadId = (short)attributeComponent[NumericType.RoadId];
            // 出生时间戳, 服务器发送毫秒
            long liveTime = attributeComponent[NumericType.LiveTime];
            // 剩余存活时间, 单位毫秒
            uint remainTime = (uint)attributeComponent[NumericType.RemainTime];
            // 初始位置偏移值 XYZ, 在这里转换进行转换, 因为转换的修正值配置在 Model 层
            float offsetPosX = (float)attributeComponent[NumericType.PositionX] / FishConfig.ServerOffsetScale;
            float offsetPosY = (float)attributeComponent[NumericType.PositionY] / FishConfig.ServerOffsetScale;
            float offsetPosZ = (float)attributeComponent[NumericType.PositionZ] / FishConfig.ServerOffsetScale;

            FishMoveHelper.InitInfo(info, roadId, liveTime, remainTime, offsetPosX, offsetPosY, offsetPosZ);
            self.Info = info;

            self.AimPoint = StructureHelper.Pop_Vector3_Class();

            self.IsInScreen = false;

            var transformComponent = unit.TransformComponent;
            transformComponent.NodeName = string.Format(FishConfig.NameFormat, unit.ConfigId, unit.UnitId);
            transformComponent.Info.Update(info);
        }
    }

    [ObjectSystem]
    public class FishUnitComponentDestroySystem : DestroySystem<FishUnitComponent>
    {
        public override void Destroy(FishUnitComponent self)
        {
            FishMoveHelper.PushPool(self.Info);
            self.Info = null;

            StructureHelper.PushPool(self.AimPoint);
            self.AimPoint = null;
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    internal static class FishUnitComponentSystem
    {
        internal static void FixedUpdate(this FishUnitComponent self, Unit unit)
        {
            FishMoveInfo info = self.Info;
            FishMoveHelper.FixedUpdate(info);

            if (!info.IsMoveEnd)
                unit.TransformComponent.Info.Update(info);
        }

        internal static void SetMoveSpeed(this FishUnitComponent self, float moveSpeed) =>
                             self.Info.MoveSpeed = moveSpeed;

        internal static void PauseMove(this FishUnitComponent self) => self.Info.IsPause = true;

        internal static void ResumeMove(this FishUnitComponent self) => self.Info.IsPause = false;
    }
}