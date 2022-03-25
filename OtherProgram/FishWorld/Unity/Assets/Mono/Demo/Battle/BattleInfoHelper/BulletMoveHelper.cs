using System;
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

        public static void InitInfo(BulletMoveInfo info, float moveSpeed, float moveDirX, float moveDirY,
                                float startPosX, float startPosY, UInt32 trackEntityId)
        {
            //_nextPos.Set(startPosX, startPosY, 0);
            //_transCom.SetLocalPos(_nextPos);
            SetMoveDirection(moveDirX, moveDirY, true);
            //StartMove(moveSpeed);
        }

        public static void FixedUpdate(BulletMoveInfo info)
        {
            //Vector3 targetPos = fishEntity.GetScreenPos();
            //Vector3 scrrenPos = _transCom.GetScreenPos();
            //float moveDirX = targetPos.x - scrrenPos.x;
            //float moveDirY = targetPos.y - scrrenPos.y;
            //if (moveDirX > DirectionFix && moveDirY > DirectionFix)
            //    SetMoveDirection(moveDirX, moveDirY, true);
            
            float moveLength = TimeHelper.ClinetDeltaFrameTime() * info.MoveSpeed;
            ref Vector2 MoveDirection = ref info.MoveDirection;
            ref Vector2 NextStep = ref info.NextStep;
            NextStep.x = info.MoveDirection.x * moveLength;
            NextStep.y = info.MoveDirection.y * moveLength;
            
            //if (EntityUtility.CheckBulletScreenReflect(ref _nextPos, ref info.MoveDirection))
            //    SetMoveDirection(info.MoveDirection.x, info.MoveDirection.y, false);
        }

        //public override void OnCustomUpdate()
        //{
        //    _transCom.SetLocalPos(_nextPos);
        //    _transCom.SetLocalRotation(NextRotation);
        //}

        public static void SetMoveDirection(float moveDirX, float moveDirY, bool isNormalize)
        {
            //info.MoveDirection.Set(moveDirX, moveDirY);
            //if (isNormalize)
            //    info.MoveDirection.Normalize();
            //EntityUtility.CalcScreenQuaternion(info.MoveDirection, ref NextRotation);
            //// 子弹本身的模型制作带了翻转
            //NextRotation.z = -NextRotation.y;
            //NextRotation.y = 0;
        }
    }
}