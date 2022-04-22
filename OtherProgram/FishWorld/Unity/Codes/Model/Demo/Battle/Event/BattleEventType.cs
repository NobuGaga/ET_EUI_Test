namespace ET
{
    namespace EventType
    {
        #region Receive Message Logic Event

        /// <summary> 接收到进入房间事件 </summary>
        public struct ReceiveEnterRoom
        {
            public Scene CurrentScene;

            public int RoomId;

            /// <summary> 冰冻技能结束时间戳 </summary>
            public long IceEndTime { get; set; }

            public int AreaId;
        }

        /// <summary> 接收到切换区域事件 </summary>
        public struct ReceiveExchangeArea
        {
            public FisheryComponent FisheryComponent;
        }

        /// <summary> 接收到 Boss 来袭事件 </summary>
        public struct ReceiveBossComming
        {
            public Scene ZoneScene;

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

        /// <summary> 接收到 Use Skill 事件 </summary>
        public struct ReceiveSkillUse
        {
            public Scene CurrentScene;

            public M2C_SkillUse Message;
        }

        #endregion

        #region Logic To UI

        /// <summary> 子弹跟鱼发生碰撞事件 </summary>
        public struct BulletCollideFish
        {
            public Scene CurrentScene;

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

        #region Logic To View

        public struct RemoveBulletUnit
        {
            public Scene CurrentScene;

            public long UnitId;
        }

        public struct AfterCreateSkillUnit
        {
            public Scene CurrentScene;

            public SkillUnit SkillUnit;
        }

        /// <summary> 渔场技能效果结束事件 </summary>
        public struct FisherySkillEnd
        {
            public Scene CurrentScene;

            public int SkillType;
        }

        #endregion
    }
}