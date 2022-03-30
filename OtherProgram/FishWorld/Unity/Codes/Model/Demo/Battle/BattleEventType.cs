namespace ET
{
    namespace EventType
    {
        #region Receive Message Logic To View

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

            public float TouchPosX;

            public float TouchPosY;

            public M2C_Fire Message;
        }

        #endregion

        #region Logic To UI

        /// <summary> 子弹跟鱼发生碰撞事件 </summary>
        public struct BulletCollideFish
        {
            public float ScreenPosX;

            public float ScreenPosY;
            
            public long PlayerUnitId;
            
            public long FishUnitId;
        }

        /// <summary> 击杀鱼事件 </summary>
        public struct KillFish
        {
            public Scene CurrentScene;

            public M2C_Hit Message;
        }

        #endregion
    }
}