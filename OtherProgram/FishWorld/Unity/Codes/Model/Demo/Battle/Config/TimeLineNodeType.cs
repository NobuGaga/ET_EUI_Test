namespace ET
{
    /// <summary> 时间轴状态类型 </summary>
    public static class TimeLineNodeType
    {
        // 时间轴类型从 1 开始
        /// <summary> 播放模型动作 </summary>
        public const int PlayAnimate = 1;

        /// <summary> 旋转 </summary>
        public const int Rotate = 2;

        /// <summary> 匀加速运动 </summary>
        public const int SpeedChange = 3;

        /// <summary> UI 出场动画 </summary>
        public const int AppearUI = 4;

        // 状态类型从 11 开始
        /// <summary> 预备状态(服务器通知刷怪, 但怪物为不可攻击状态(可能跟场景融为一体之类的)) </summary>
        public const int ReadyState = 11;

        /// <summary> 活跃状态(正常可攻击状态) </summary>
        public const int ActiveState = 12;

        /// <summary> 非活跃状态(正常不可攻击状态) </summary>
        public const int InactiveState = 13;

        /// <summary> 死亡状态 </summary>
        public const int DeadState = 14;
    }
}