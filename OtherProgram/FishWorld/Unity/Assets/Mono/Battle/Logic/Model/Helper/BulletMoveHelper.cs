using UnityEngine;

namespace ET
{
    /// <summary>
    /// 子弹移动辅助类, 对应 ILRuntime 层 BulletMoveComponent 调用方法
    /// 用于处理复杂的计算问题
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class BulletMoveHelper
    {
        public static BulletMoveInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<BulletMonoUnit>(unitId);
            var info = unit.BulletMoveInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(BulletMoveInfo)) as BulletMoveInfo;
            unit.BulletMoveInfo = info;
            return info;
        }

        public static void PushPool(long unitId, BulletMoveInfo info)
        {
            var unit = UnitMonoComponent.Instance.Get<BulletMonoUnit>(unitId);
            if (unit != null)
                unit.BulletMoveInfo = null;

            MonoPool.Instance.Recycle(info);
        }

        public static void InitInfo(BulletMoveInfo bulletMoveInfo, CannonShootInfo cannonShootInfo)
        {
            bulletMoveInfo.CurrentLocalPos = cannonShootInfo.GetBulletStartPosition();
            bulletMoveInfo.MoveDirection = cannonShootInfo.ShootDirection;
            SetNextRotation(bulletMoveInfo);
        }

        public static void FixedUpdate(BulletMoveInfo info)
        {
            // 反弹逻辑修改, 先判断上一次渲染的位置是否到达边界(也就是先显示碰到边界)
            // 再修改射击方向, 设置当前帧反弹后的位置
            BulletCameraHelper.CheckCollideBorder(info);

            ref Vector3 currentLocalPos = ref info.CurrentLocalPos;
            ref Vector2 moveDirection = ref info.MoveDirection;

            float moveLength = TimeHelper.ClinetDeltaFrameTime() * info.MoveSpeed;
            currentLocalPos.x += moveDirection.x * moveLength;
            currentLocalPos.y += moveDirection.y * moveLength;

            SetNextRotation(info);
        }

        private static void SetNextRotation(BulletMoveInfo info)
        {
            CannonShootHelper.SetLocalQuaternion(info);

            // Battle Warning 子弹本身的模型制作带了翻转
            info.CurrentRotation.z = - info.CurrentRotation.y;
            info.CurrentRotation.y = 0;
        }
    }
}