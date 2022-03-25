using UnityEngine;

namespace ET
{
    /// <summary>
    /// 炮台射击辅助类, 
    /// 
    /// 
    /// 
    /// 对应 ILRuntime 层 BulletMoveComponent 调用方法
    /// 
    /// 
    /// 
    /// 用于处理复杂的计算问题
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class CannonShootHelper
    {
        public static CannonShootInfo PopInfo() =>
                        MonoPool.Instance.Fetch(typeof(CannonShootInfo)) as CannonShootInfo;

        //private static readonly float forwardX = Vector3.forward.x;
        //private static readonly float forwardY = Vector3.forward.y;
        //private static readonly float forwardZ = Vector3.forward.z;

        //public static void GetCannonShootDirection(CannonDirectionInfo info)
        //{
        //    ref Quaternion rotation = ref info.LocalRotation;
        //    float doubleX = rotation.x * 2;
        //    float doubleY = rotation.y * 2;
        //    float doubleZ = rotation.z * 2;
        //    float squareX = rotation.x * doubleX;
        //    float squareY = rotation.y * doubleY;
        //    float squareZ = rotation.z * doubleZ;
        //    float multiplyXY = rotation.x * doubleY;
        //    float multiplyXZ = rotation.x * doubleZ;
        //    float multiplyYZ = rotation.y * doubleZ;
        //    float multiplyWX = rotation.w * doubleX;
        //    float multiplyWY = rotation.w * doubleY;
        //    float multiplyWZ = rotation.w * doubleZ;

        //    ref Vector2 direction = ref info.ShootDirection;
        //    direction.x = ((1 - squareY + squareZ) * forwardX) + ((multiplyXY - multiplyWZ) * forwardY) + ((multiplyXZ + multiplyWY) * forwardZ);
        //    direction.y = ((multiplyXZ - multiplyWY) * forwardX) + ((multiplyYZ + multiplyWX) * forwardY) + ((1 - (squareX + squareY)) * forwardZ);
        //}

        /// <summary>
        /// 获取炮台射击方向
        /// </summary>
        /// <param name="rotation">炮台 local 旋转角度</param>
        public static void InitInfo(CannonShootInfo info)
        {
            ref Quaternion rotation = ref info.LocalRotation;
            float doubleY = rotation.y * 2;
            ref Vector2 direction = ref info.ShootDirection;
            direction.x = rotation.x * rotation.z * 2 + rotation.w * doubleY;
            direction.y = 1 - (rotation.x * rotation.x * 2 + rotation.y * doubleY);

            ref Vector3 ShootScreenPosition = ref info.ShootScreenPosition;
            ShootScreenPosition.x /= CameraHelper.WidthRatio;
            ShootScreenPosition.y /= CameraHelper.HeightRatio;
        }
    }
}