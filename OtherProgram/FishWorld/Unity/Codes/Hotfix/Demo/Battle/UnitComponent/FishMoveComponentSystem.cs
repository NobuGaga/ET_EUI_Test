namespace ET
{
    [ObjectSystem]
    public class FishMoveComponentAwakeSystem : AwakeSystem<FishMoveComponent, int>
    {
        public override void Awake(FishMoveComponent self, int ConfigId)
        {
            self.ConfigId = (ushort)ConfigId;

            FishMoveInfo info = FishMoveInfo.PopInfo();
            info.Reset();

            // 这里用 var 看起来不像 NumericComponent 组件 = =
            var attributeComponent = self.AttributeComponent();
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

            self.BattleUnitLogicComponent().IsUpdate = !info.IsMoveEnd;
        }
    } 

    [ObjectSystem]
    public class FishMoveComponentDestroySystem : DestroySystem<FishMoveComponent>
    {
        public override void Destroy(FishMoveComponent self) => self.Info.PushPool();
    }

    #region Component Getter

    /// <summary> 定义 FishMoveComponent 获取父组件方法 </summary>
    public static class FishMoveComponentParentComponentSystem
    {
        public static BattleUnitLogicComponent BattleUnitLogicComponent(this FishMoveComponent self)
                                                            => self.Parent as BattleUnitLogicComponent;

        public static NumericComponent AttributeComponent(this FishMoveComponent self)
                                                => self.BattleUnitLogicComponent().AttributeComponent();
    }

    #endregion

    public static class FishMoveComponentSystem
    {
        public static void FixedUpdate(this FishMoveComponent self)
        {
            FishMoveInfo info = self.Info;
            FishMoveHelper.FixedUpdate(info);
            if (info.IsTimeOutMove())
                info.NextPos = FishConfig.RemovePoint;
            self.UpdateTransform();
        }

        public static void UpdateTransform(this FishMoveComponent self)
        {
            TransformComponent transformComponent = self.BattleUnitLogicComponent().TransformComponent();
            FishMoveInfo info = self.Info;
            transformComponent.SetLocalPos(info.NextPos);
            transformComponent.SetForward(info.NextForward);
        }

        public static string GetNodeName(this FishMoveComponent self) =>
                string.Format(FishConfig.NameFormat, self.ConfigId, self.BattleUnitLogicComponent().UnitId);
    }
}