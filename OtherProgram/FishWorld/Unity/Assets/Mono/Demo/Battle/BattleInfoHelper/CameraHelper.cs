using UnityEngine;

namespace ET
{
    /// <summary>
    /// 摄像机计算相关方法
    /// </summary>
    public static class CameraHelper
    {
        private static Vector2 cannonBorder;

        public static float WidthRatio;
        public static float HeightRatio;

        /// <summary> 初始化使用传参的形式, 因为要被 Robot 层引用 </summary>
        public static void Init(int screenWidth, int screenHeight, float orthographicSize)
        {
            float ratio = (float)screenWidth / screenHeight;

            //cannonBorder.Set(orthographicSize * ratio * 2, orthographicSize * 2);

            WidthRatio = screenWidth / orthographicSize / ratio / 2;
            HeightRatio = screenHeight / orthographicSize / 2;
        }

        public static bool CheckBulletScreenReflect(Vector3 bulletPos, ref Vector2 moveDir)
        {
            bool isReflect = false;
            //if (bulletPos.IsLessZeroX() || bulletPos.IsGreaterX(cannonBorder.x))
            //{
            //    isReflect = true;
            //    bulletPos.x = Mathf.Clamp(bulletPos.x, 0, cannonBorder.x);
            //    moveDir.x = -moveDir.x;
            //}
            //if (bulletPos.IsLessZeroY() || bulletPos.IsGreaterY(cannonBorder.y))
            //{
            //    isReflect = true;
            //    bulletPos.y = Mathf.Clamp(bulletPos.y, 0, cannonBorder.y);
            //    moveDir.y = -moveDir.y;
            //}
            return isReflect;
        }
    }
}