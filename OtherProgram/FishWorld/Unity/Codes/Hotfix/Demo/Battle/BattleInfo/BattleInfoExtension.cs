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
            self.LogicPos = TransformDefaultConfig.DefaultPosition;
            self.LogicRotation = TransformDefaultConfig.DefaultRotation;
            self.LogicScale = TransformDefaultConfig.DefaultScale;
            self.LogicForward = TransformDefaultConfig.DefaultForward;
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
        }
    }
}