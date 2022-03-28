namespace ET
{
    namespace EventType
    {
        #region Battle Logic To View Event

        /// <summary> 接收到进入房间事件 </summary>
        public struct ReceiveEnterRoom
        {
            public Scene CurrentScene;

            public FisheryComponent FisheryComponent;
        }

        /// <summary> 接收到切换区域事件 </summary>
        public struct ReceiveExchangeArea
        {
            public FisheryComponent FisheryComponent;
        }

        /// <summary> 接收到 Boss 来袭事件 </summary>
        public struct ReceiveBossComming
        {
            public int BossUnitConfigId;
        }

        /// <summary> 接收到 Fire 事件 </summary>
        public struct ReceiveFire
        {
            public Scene CurrentScene;

            public UnitInfo UnitInfo;

            public float ShootDirX;

            public float ShootDirY;

            public M2C_Fire Message;
        }

        #endregion
    }
}