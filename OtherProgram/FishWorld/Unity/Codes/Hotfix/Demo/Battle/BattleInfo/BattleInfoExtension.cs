namespace ET
{
    /// <summary>
    /// 跟 Mono 层交互的数据结构拓展方法在这里定义
    /// 使用 Extension 后缀跟 Model 层数据结构区分, 这是在 Mono 层定义的数据结构(类)
    /// </summary>
    public static class BattleInfoExtension
    {
        public static void Reset(this BulletMoveInfo self)
        {
            self.CurrentRotation = TransformDefaultConfig.DefaultRotation;

            self.MoveSpeed = BulletConfig.DefaultMoveSpeed;
            self.MoveDirection = BulletConfig.DefaultMoveDirection;

            self.CurrentLocalPos = BulletConfig.RemovePoint;

            self.TrackPosition = BulletMoveDefaultInfo.TrackPosition;
        }

        public static void Reset(this FishMoveInfo self)
        {
            self.IsPause = false;
            self.IsMoveEnd = false;
            
            self.MoveTime = 0;
            self.MoveDuration = 0;

            self.OffsetPosX = 0;
            self.OffsetPosY = 0;
            self.OffsetPosZ = 0;

            self.NextPos = FishConfig.RemovePoint;
            self.NextForward = TransformDefaultConfig.DefaultForward;

            self.MoveSpeed = FishConfig.DefaultMoveSpeed;

            self.RoadId = 0;
            self.IsPathMove = false;
        }
    }
}