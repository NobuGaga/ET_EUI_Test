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
        public class ReceiveFire : DisposeObject
        {
            public Scene CurrentScene;

            public UnitInfo UnitInfo;

            public float TouchPosX;

            public float TouchPosY;

            public M2C_Fire Message;

            public static ReceiveFire Instance = new ReceiveFire();

            public override void Dispose()
            {
                CurrentScene = null;
                UnitInfo = null;
                Message = null;
            }
        }

        /// <summary> 接收到 Use Skill 事件 </summary>
        public class ReceiveSkillUse : DisposeObject
        {
            public Scene CurrentScene;

            public M2C_SkillUse Message;

            public static ReceiveSkillUse Instance = new ReceiveSkillUse();

            public override void Dispose()
            {
                CurrentScene = null;
                Message = null;
            }
        }

        #endregion

        #region Logic To UI

        /// <summary> 子弹跟鱼发生碰撞事件 </summary>
        public class BulletCollideFish : DisposeObject
        {
            public Scene CurrentScene;

            public float ScreenPosX;

            public float ScreenPosY;
            
            public long PlayerUnitId;
            
            public long FishUnitId;

            public static BulletCollideFish Instance = new BulletCollideFish();

            public override void Dispose() => CurrentScene = null;
        }

        /// <summary> 击杀鱼事件 </summary>
        public class KillFish : DisposeObject
        {
            public Scene CurrentScene;

            public M2C_Hit Message;

            public static KillFish Instance = new KillFish();

            public override void Dispose()
            {
                CurrentScene = null;
                Message = null;
            }
        }

        #endregion

        #region Logic To View

        public class RemoveBulletUnit : DisposeObject
        {
            public Scene CurrentScene;

            public long UnitId;

            public static RemoveBulletUnit Instance = new RemoveBulletUnit();

            public override void Dispose()
            {
                CurrentScene = null;
                UnitId = 0;
            }
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