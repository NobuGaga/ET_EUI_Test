namespace ET
{
    /// <summary>
    /// 跟 Mono 层交互的数据结构拓展方法在这里定义
    /// 使用 Extension 后缀跟 Model 层数据结构区分, 这是在 Mono 层定义的数据结构(类)
    /// </summary>
    public static class BattleInfoExtension
    {
        public static void Reset(this TransformInfo self)
        {
            self.WorldPosition = TransformDefaultConfig.DefaultPosition;
            self.LocalPosition = TransformDefaultConfig.DefaultPosition;
            self.Rotation = TransformDefaultConfig.DefaultRotation;
            self.LocalRotation = TransformDefaultConfig.DefaultRotation;
            self.Forward = TransformDefaultConfig.DefaultForward;
        }

        public static void Reset(this BulletMoveInfo self)
        {
            self.CurrentRotation = TransformDefaultConfig.DefaultRotation;

            self.MoveSpeed = BulletConfig.DefaultMoveSpeed;
            self.MoveDirection = BulletConfig.DefaultMoveDirection;

            self.CurrentLocalPos = BulletConfig.RemovePoint;
            self.ScreenPos = TransformDefaultConfig.DefaultScreenPos;
        }

        /// <summary> 使用热更层设定默认数值重置 </summary>
        public static void Reset(this FishMoveInfo self)
        {
            self.NextPos = FishConfig.RemovePoint;
            self.NextForward = TransformDefaultConfig.DefaultForward;
            self.MoveSpeed = FishConfig.DefaultMoveSpeed;
            self.MoveAcceleration = FishConfig.DefaultMoveAcceleration;
            self.IsPause = false;
        }

        public static void ResetRotateInfo(this TransformRotateInfo self)
        {
            self.LocalRotationZ = 0;
            self.RotationDuration = 0;
            self.RotationTime = 0;
        }

        public static void ResetForward(this TransformRotateInfo self) =>
                           self.IsFowardMainCamera = false;
    }
}