using UnityEngine;

namespace ET
{
    /// <summary>
    /// 子弹摄像机计算相关方法
    /// </summary>
    public static class BulletCameraHelper
    {
        /// <summary> 检查子弹是否跟边界进行碰撞 </summary>
        public static bool CheckCollideBorder(BulletMoveInfo info)
        {
            bool isReflect = false;
            ref Vector3 bulletLocalPos = ref info.CurrentLocalPos;
            ref Vector2 moveDirection = ref info.MoveDirection;

            ref Vector2 cannonBorder = ref ConstHelper.CannonBorder;

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
    }
}