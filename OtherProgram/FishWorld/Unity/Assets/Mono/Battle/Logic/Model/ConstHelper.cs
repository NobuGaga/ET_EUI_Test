
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

        public static Vector2 CannonBorder;

        public static float CannonCameraWidthRatio;
        public static float CannonCameraHeightRatio;

        /// <summary> 初始化使用传参的形式在 Mono 层调用, 因为要被 Robot 层引用 </summary>
        public static void Init(int screenWidth, int screenHeight, float orthographicSize)
        {
            float ratio = (float)screenWidth / screenHeight;
            CannonBorder.x = orthographicSize * ratio * 2;
            CannonBorder.y = orthographicSize * 2;
            
            CannonCameraWidthRatio = screenWidth / orthographicSize / ratio / 2;
            CannonCameraHeightRatio = screenHeight / orthographicSize / 2;
        }
    }
}