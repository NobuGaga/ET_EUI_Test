using UnityEngine;

namespace ET
{
    // 战斗用常量代码配置
    public static class ConstHelper
    {
        public const ushort FisheryUnitCount = 2 ^ 9;
        public const ushort FisheryBulletCount = 2 ^ 8;

        public const string CannonCameraNodeName = "/Global/CannonCamera";
        public const string FishRootNodeName = "/Global/FishRoot";

        /// <summary> 默认子弹追踪的鱼 Unit ID </summary>
        public const long DefaultTrackFishUnitId = 0;

        /// <summary> 预创建热更层鱼相关类数量 </summary>
        public const int PreCreateFishClassCount = 300;

        /// <summary> 追踪速度方向修正值, 小于这个值则用回之前的速度值 </summary>
        public const float TrackDirectionFix = 0.1f;

        /// <summary> 鱼预设种类 </summary>
        public const float FishModelCount = 2 ^ 8;

        /// <summary> 时间轴类型, 活跃状态(正常可攻击状态) </summary>
        public static int TimeLineType_ActiveState = 12;

        /// <summary> 时间轴类型, 死亡状态 </summary>
        public static int TimeLineType_DeadState = 14;

        public const char MotionSymbol = '@';

        public const string MotionSymbolString = "@";

        public static bool IsEditor;

        public static Vector2 CannonBorder;

        public static float CannonCameraWidthRatio;
        public static float CannonCameraHeightRatio;

        /// <summary> 初始化使用传参的形式在 Mono 层调用, 因为要被 Robot 层引用 </summary>
        public static void Init(bool isEditor, int screenWidth, int screenHeight, float orthographicSize)
        {
            IsEditor = isEditor;

            float ratio = (float)screenWidth / screenHeight;
            CannonBorder.x = orthographicSize * ratio * 2;
            CannonBorder.y = orthographicSize * 2;
            
            CannonCameraWidthRatio = screenWidth / orthographicSize / ratio / 2;
            CannonCameraHeightRatio = screenHeight / orthographicSize / 2;
        }

        public static void InitTimeLineConfig(int activeState, int deadState)
        {
            TimeLineType_ActiveState = activeState;
            TimeLineType_DeadState = deadState;
        }
    }
}