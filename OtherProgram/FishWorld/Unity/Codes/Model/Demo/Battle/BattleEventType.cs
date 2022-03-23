namespace ET
{
    namespace EventType
    {
        /// <summary> 接收到进入房间事件 </summary>
        public struct AfterEnterRoom
        {
            public Scene CurrentScene;

            public FisheryComponent FisheryComponent;
        }

        /// <summary> 接收到切换区域事件 </summary>
        public struct AfterExchangeArea
        {
            public FisheryComponent FisheryComponent;
        }

        /// <summary> 接收到 Boss 来袭事件 </summary>
        public struct AfterBossComming
        {
            public int BossUnitConfigId;
        }
    }
}