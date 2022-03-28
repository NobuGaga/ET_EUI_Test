namespace ET
{
	public static class FisheryConfig
    {
        /// <summary> 错误座位码 </summary>
        public const short ErrorSeatId = -1;

        /// <summary> 座位上限 </summary>
        public const ushort SeatCount = 4;

        /// <summary> 渔场最大子弹上限 </summary>
        public const ushort FisheryMaxBulletCount = SeatCount * BulletConfig.ShootMaxBulletCount;
    }
}