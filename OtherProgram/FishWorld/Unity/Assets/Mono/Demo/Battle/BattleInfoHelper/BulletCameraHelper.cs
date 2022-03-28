using UnityEngine;

namespace ET
{
    /// <summary>
    /// 子弹摄像机计算相关方法
    /// </summary>
    public static class BulletCameraHelper
    {
        private static Vector2 cannonBorder;

        public static float WidthRatio;
        public static float HeightRatio;

        /// <summary> 初始化使用传参的形式在 Mono 层调用, 因为要被 Robot 层引用 </summary>
        public static void Init(int screenWidth, int screenHeight, float orthographicSize)
        {
            float ratio = (float)screenWidth / screenHeight;

            cannonBorder.Set(orthographicSize * ratio * 2, orthographicSize * 2);

            WidthRatio = screenWidth / orthographicSize / ratio / 2;
            HeightRatio = screenHeight / orthographicSize / 2;
        }

        /// <summary> 检查子弹是否跟边界进行碰撞 </summary>
        public static bool CheckCollideBorder(BulletMoveInfo info)
        {
            bool isReflect = false;
            ref Vector3 bulletLocalPos = ref info.CurrentLocalPos;
            ref Vector2 moveDirection = ref info.MoveDirection;

            if (bulletLocalPos.x < 0 || bulletLocalPos.x > cannonBorder.x)
            {
                isReflect = true;
                bulletLocalPos.x = Mathf.Clamp(bulletLocalPos.x, 0, cannonBorder.x);
                moveDirection.x = -moveDirection.x;
            }

            if (bulletLocalPos.y < 0 || bulletLocalPos.y > cannonBorder.y)
            {
                isReflect = true;
                bulletLocalPos.y = Mathf.Clamp(bulletLocalPos.y, 0, cannonBorder.y);
                moveDirection.y = -moveDirection.y;
            }

            moveDirection.Normalize();

            return isReflect;
        }

        private static void Set(this ref Vector2 self, float x, float y)
        {
            self.x = x; self.y = y;
        }
    }
}