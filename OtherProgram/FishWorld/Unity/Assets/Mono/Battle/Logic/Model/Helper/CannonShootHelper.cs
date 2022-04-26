using UnityEngine;

namespace ET
{
    /// <summary>
    /// 炮台射击辅助类
    /// 对应 ILRuntime 层 CallLogicShootBullet 调用方法
    /// 用于处理复杂的计算问题
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class CannonShootHelper
    {
        /// <summary>
        /// 从对象池中获取炮台射击信息
        /// </summary>
        /// <param name="localRotation">炮台上一帧旋转角度</param>
        /// <param name="shootPointScreenPos">射击点的屏幕位置</param>
        /// <returns></returns>
        public static CannonShootInfo PopInfo() =>
                        MonoPool.Instance.Fetch(typeof(CannonShootInfo)) as CannonShootInfo;

        public static void PushPool(CannonShootInfo info) => MonoPool.Instance.Recycle(info);

        public static void InitInfo(CannonShootInfo info, Vector3 shootPointScreenPos)
        {
            info.ShootPointScreenPos = shootPointScreenPos;
            ref Vector3 shootScreenPosition = ref info.ShootPointScreenPos;
            ref Vector2 shootLocalPosition = ref info.ShootLocalPosition;
            shootLocalPosition.x = shootScreenPosition.x / ConstHelper.CannonCameraWidthRatio;
            shootLocalPosition.y = shootScreenPosition.y / ConstHelper.CannonCameraHeightRatio;
        }

        public static void SetLocalQuaternion(CannonShootInfo info, float shootDirectionX, float shootDirectionY)
        {
            info.ShootDirection.x = shootDirectionX;
            info.ShootDirection.y = shootDirectionY;
            info.ShootDirection.Normalize();
            SetLocalQuaternion(ref info.LocalRotation, ref info.ShootDirection);
        }

        public static void SetLocalQuaternion(BulletMoveInfo info) =>
                           SetLocalQuaternion(ref info.CurrentRotation, ref info.MoveDirection);

        /// <summary>
        /// 计算炮台或者子弹的旋转, 炮台和子弹公用一个摄像机, 局部坐标系相同
        /// 热更层或者 Mono 层调用传参使用类数据结构体, 实际实现调用内部的 ref 传参函数
        /// 因为热更层不用使用 ref 调用 Mono 层函数, 而数据最终会被热更层引用
        /// 计算子弹的旋转不需要依赖原来子弹的旋转, 旋转依赖方向为水平向上(Vector2.up)
        /// 这里传入的旋转只是用来赋值
        /// </summary>
        /// <param name="localRotation">局部旋转</param>
        /// <param name="shootDirection">射击方向, 单位向量</param>
        private static void SetLocalQuaternion(ref Quaternion localRotation, ref Vector2 shootDirection)
        {
            //float dotShoot = Vector2.Dot(Vector2.up, shootDirection);
            float dotShoot = shootDirection.y;

            if (dotShoot > 0.999)
            {
                localRotation.Set(0, 0, 0, 1);
                return;
            }

            if (dotShoot < -0.999)
            {
                localRotation.Set(0, 1, 0, 0);
                return;
            }

            float qw = Mathf.Sqrt(1 * shootDirection.SqrMagnitude()) + dotShoot;
            localRotation.Set(0, shootDirection.x, 0, qw);
            localRotation.Normalize();
        }

        private static void Set(this ref Quaternion self, float x, float y, float z, float w)
        {
            self.x = x; self.y = y; self.z = z; self.w = w;
        }

        private static float SqrMagnitude(this ref Vector2 self) => self.x * self.x + self.y * self.y;
    }
}