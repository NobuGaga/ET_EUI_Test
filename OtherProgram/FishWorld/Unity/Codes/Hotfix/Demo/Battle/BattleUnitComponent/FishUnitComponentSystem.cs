// Battle Review Before Boss Node

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

            FishMoveInfo moveInfo = FishMoveInfoHelper.PopInfo(unit.UnitId);
            moveInfo.Reset();

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
            int originConfigSpeed = attributeComponent.GetAsInt(NumericType.Speed);

            FishMoveInfoHelper.InitInfo(moveInfo, roadId, liveTime, remainTime, offsetPosX, offsetPosY, offsetPosZ, originConfigSpeed);
            self.MoveInfo = moveInfo;

            self.ScreenInfo = FishScreenInfoHelper.PopInfo(unit.UnitId);
            self.ScreenInfo.IsInScreen = false;

            var transformComponent = unit.TransformComponent;
            transformComponent.Info.Update(moveInfo);

            TransformRotateInfo rotateInfo = TransformRotateInfoHelper.PopInfo(unit.UnitId);
            rotateInfo.ResetRotateInfo();
            rotateInfo.ResetForward();
            self.RotateInfo = rotateInfo;

            int timeLineConfigId = attributeComponent.GetAsInt(NumericType.Timeline);

            TimeLineConfigInfoHelper.SetConfigId(unit.UnitId, timeLineConfigId);

            // Battle TODO delete
            int fishGroupId = attributeComponent.GetAsInt(NumericType.GroupUid);
            Log.Debug($"创建鱼 鱼组表 ID = { fishGroupId }, 基础表 ID = { unit.ConfigId }, 鱼线 ID = { roadId }, 时间轴 ID = { timeLineConfigId }");
        }
    }

    [ObjectSystem]
    public class FishUnitComponentDestroySystem : DestroySystem<FishUnitComponent>
    {
        public override void Destroy(FishUnitComponent self)
        {
            long unitId = self.Parent.Id;
            var moveInfo = self.MoveInfo;
            self.MoveInfo = null;
            FishMoveInfoHelper.PushPool(unitId, moveInfo);

            var screenInfo = self.ScreenInfo;
            self.ScreenInfo = null;
            FishScreenInfoHelper.PushPool(unitId, screenInfo);

            var rotateInfo = self.RotateInfo;
            self.RotateInfo = null;
            TransformRotateInfoHelper.PushPool(unitId, rotateInfo);
        }
    }

    [FriendClass(typeof(FishUnitComponent))]
    public static class FishUnitComponentSystem
    {
        internal static void SetMoveSpeed(this FishUnitComponent self, float moveSpeed) =>
                             self.MoveInfo.MoveSpeed = moveSpeed;

        public static void PauseMove(this FishUnitComponent self) => self.MoveInfo.IsPause = true;

        public static void ResumeMove(this FishUnitComponent self) => self.MoveInfo.IsPause = false;
    }
}