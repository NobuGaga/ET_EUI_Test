namespace ET
{
    [ObjectSystem]
    public class FishUnitComponentAwakeSystem : AwakeSystem<FishUnitComponent>
    {
        public override void Awake(FishUnitComponent self)
        {
            Unit unit = self.Parent as Unit;

            FishMoveInfo info = FishMoveHelper.PopInfo();
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

            self.AimPoint = StructureHelper.Pop_Vector3_Class();

            self.InitTransform();
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

    internal static class FishUnitComponentSystem
    {
        internal static void FixedUpdate(this FishUnitComponent self)
        {
            FishMoveInfo info = self.Info;
            FishMoveHelper.FixedUpdate(info);
            
            if (!info.IsMoveEnd)
            {
                self.UpdateTransform();
                return;
            }

            Scene currentScene = self.DomainScene();
            var battleLogicComponent = currentScene.GetBattleLogicComponent();
            battleLogicComponent.RemoveUnitIdList.Add(self.Parent.Id);
        }

        internal static void InitTransform(this FishUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            transformComponent.NodeName = self.GetNodeName();
            self.UpdateTransform();
        }

        private static void UpdateTransform(this FishUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            FishMoveInfo info = self.Info;
            transformComponent.SetLocalPos(info.NextPos);
            transformComponent.SetForward(info.NextForward);
        }

        private static string GetNodeName(this FishUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            return string.Format(FishConfig.NameFormat, unit.ConfigId, unit.Id);
        }

        internal static void SetMoveSpeed(this FishUnitComponent self, float moveSpeed) => self.Info.MoveSpeed = moveSpeed;

        internal static void PauseMove(this FishUnitComponent self) => self.Info.IsPause = true;

        internal static void ResumeMove(this FishUnitComponent self) => self.Info.IsPause = false;
    }
}