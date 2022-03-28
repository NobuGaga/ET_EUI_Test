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
        public static BulletMoveInfo PopInfo() =>
                                    MonoPool.Instance.Fetch(typeof(BulletMoveInfo)) as BulletMoveInfo;

        public static void PushPool(BulletMoveInfo info) => MonoPool.Instance.Recycle(info);

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

            float moveLength = TimeHelper.ClinetDeltaFrameTime() * info.MoveSpeed;
            ref Vector2 MoveDirection = ref info.MoveDirection;
            ref Vector3 CurrentLocalPos = ref info.CurrentLocalPos;
            CurrentLocalPos.x += info.MoveDirection.x * moveLength;
            CurrentLocalPos.y += info.MoveDirection.y * moveLength;

            SetNextRotation(info);

            // 追踪鱼重新计算方向, 在逻辑层设置好方向
            //Vector3 targetPos = fishEntity.GetScreenPos();
            //Vector3 scrrenPos = _transCom.GetScreenPos();
            //float moveDirX = targetPos.x - scrrenPos.x;
            //float moveDirY = targetPos.y - scrrenPos.y;
            //if (moveDirX > DirectionFix && moveDirY > DirectionFix)
            //    SetNextRotation(moveDirX, moveDirY, true);
        }

        //public override void OnCustomUpdate()
        //{
        //    _transCom.SetLocalPos(_nextPos);
        //    _transCom.SetLocalRotation(NextRotation);
        //}

        private static void SetNextRotation(BulletMoveInfo info)
        {
            CannonShootHelper.SetLocalQuaternion(info);

            // Battle Warning 子弹本身的模型制作带了翻转
            info.CurrentRotation.z = - info.CurrentRotation.y;
            info.CurrentRotation.y = 0;
        }
    }
}