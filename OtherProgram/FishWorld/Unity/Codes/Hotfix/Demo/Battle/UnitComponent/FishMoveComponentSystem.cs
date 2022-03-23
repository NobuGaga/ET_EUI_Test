namespace ET
{
    [ObjectSystem]
    public class FishMoveComponentAwakeSystem : AwakeSystem<FishMoveComponent>
    {
        public override void Awake(FishMoveComponent self)
        {
            Unit unit = self.Parent as Unit;

            FishMoveInfo info = FishMoveInfo.PopInfo();
            info.Reset();

            // 这里用 var 看起来不像 NumericComponent 组件 = =
            var attributeComponent = unit.GetComponent<NumericComponent>();
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
            self.UpdateTransform();

            unit.GetComponent<BattleUnitLogicComponent>().IsUpdate = !info.IsMoveEnd;
        }
    } 

    [ObjectSystem]
    public class FishMoveComponentDestroySystem : DestroySystem<FishMoveComponent>
    {
        public override void Destroy(FishMoveComponent self)
        {
            self.Info.PushPool();
            self.Info = null;
        }
    }

    public static class FishMoveComponentSystem
    {
        public static void FixedUpdate(this FishMoveComponent self)
        {
            FishMoveInfo info = self.Info;
            FishMoveHelper.FixedUpdate(info);
            
            if (info.IsMoveTimeOut())
                info.NextPos = FishConfig.RemovePoint;

            self.UpdateTransform();
        }

        public static void UpdateTransform(this FishMoveComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            FishMoveInfo info = self.Info;
            transformComponent.SetLocalPos(info.NextPos);
            transformComponent.SetForward(info.NextForward);
        }

        public static string GetNodeName(this FishMoveComponent self)
        {
            Unit unit = self.Parent as Unit;
            return string.Format(FishConfig.NameFormat, unit.ConfigId, unit.Id);
        }
    }
}