using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动辅助类, 对应 ILRuntime 层 FishMoveComponent 调用方法
    /// 用于处理复杂的计算问题
    /// 私有方法使用静态拓展, 共有方法使用传参
    /// </summary>
    public static class FishMoveHelper
    {
        #region Base

        private static bool IsNotValidMove(this FishMoveInfo info) => info.IsMoveEnd || info.IsPause;

        public static bool IsTimeOutMove(this FishMoveInfo info) => info.MoveTime > info.MoveDuration;

        private static FishPathInfo Path(this FishMoveInfo info) =>
                                        info.RoadId > 0 ? FishPathHelper.GetPath(info.RoadId) : null;

        private static void SetNextPosition(this FishMoveInfo info, float percentFloat)
        {
            FishPathInfo path = info.Path();
            if (path == null)
                return;

            ref Vector3 nextPos = ref info.NextPos;
            path.SetPoint(percentFloat, ref nextPos);
            nextPos.x += info.OffsetPosX;
            nextPos.y += info.OffsetPosY;
            nextPos.z += info.OffsetPosZ;
            //nextPos.Set(nextPos.x + info.OffsetPosX, nextPos.y + info.OffsetPosY, nextPos.z + info.OffsetPosZ);
        }

        private static void SetForward(this FishMoveInfo info, float percentFloat)
        {
            FishPathInfo path = info.Path();
            if (path == null)
                return;

            ref Vector3 nextPos = ref info.NextPos;
            path.SetForward(percentFloat, nextPos.x - info.OffsetPosX, nextPos.y - info.OffsetPosY,
                                            nextPos.z - info.OffsetPosZ, ref info.NextForward);
        }

        #endregion

        /// <summary>
        /// 使用服务器数据初始化移动数据
        /// </summary>
        /// <param name="info">移动数据</param>
        /// <param name="roadId">鱼线表数据</param>
        /// <param name="liveTime">出生时间戳(毫秒)</param>
        /// <param name="remainTime">剩余存活时间(秒)</param>
        /// <param name="offsetPosX">初始位置偏移值 X</param>
        /// <param name="offsetPosY">初始位置偏移值 Y</param>
        /// <param name="offsetPosZ">初始位置偏移值 Z</param>
        public static void InitInfo(FishMoveInfo info, short roadId, long liveTime, uint remainTime,
                                                float offsetPosX, float offsetPosY, float offsetPosZ)
        {
            info.IsPause = false;
            info.IsMoveEnd = false;

            // int 最大值在 20+ 天左右
            info.MoveDuration = remainTime;
            info.MoveTime = (int)(TimeHelper.ServerFrameTime() - liveTime);
            if (info.MoveTime > 0)
                info.MoveDuration += (uint)info.MoveTime;

            if (info.IsTimeOutMove())
            {
                StopMove(info);
                return;
            }

            info.RoadId = roadId;

            info.OffsetPosX = offsetPosX;
            info.OffsetPosY = offsetPosY;
            info.OffsetPosZ = offsetPosZ;

            if (info.MoveTime >= 0)
            {
                float nextTime = (float)info.MoveTime / info.MoveDuration;
                if (nextTime > 1)
                    nextTime = 1;

                info.SetForward(nextTime);

                nextTime = (float)(info.MoveTime + 30) / info.MoveDuration;
                if (nextTime > 1)
                    nextTime = 1;

                info.SetNextPosition(nextTime);
            }
            else
            {
                // 初始化 FishMoveInfo 的时候设置了 RemovePoint, 因为 RemovePoint 在热更层配置
                // 所以不在这里进行设置
                //_transCom.SetLocalPos(EntityUtility.GetFishRemovePoint());
            }

            // 调用 InitInfo 前设置, 常量标记在 Model 层
            //info.MoveSpeed = DefaultMoveSpeed;
        }

        public static void FixedUpdate(FishMoveInfo info)
        {
            if (info.IsNotValidMove())
                return;

            if (info.IsTimeOutMove())
            {
                StopMove(info);
                return;
            }

            info.MoveTime += (int)(info.MoveSpeed * TimeHelper.ClinetDeltaFrameTime());
            if (info.MoveTime < 0 || info.Path() == null)
                return;
            info.UpdateNextPosAndForward();
        }

        private static void UpdateNextPosAndForward(this FishMoveInfo info)
        {
            float nextTime = (float)info.MoveTime / info.MoveDuration;
            if (nextTime > 1)
                nextTime = 1;

            info.SetForward(nextTime);
            info.SetNextPosition(nextTime);
        }

        public static void StopMove(FishMoveInfo info)
        {
            // Battle Warning 在 FishMoveComponent.FixedUpdate 中实现设置逻辑
            //_transCom.SetLocalPos(EntityUtility.GetFishRemovePoint());
            if (info.IsMoveEnd)
                return;

            info.IsMoveEnd = true;
            info.IsPause = false;
        }

        public static void PauseMove(FishMoveInfo info) => info.IsPause = true;

        public static void ResumeMove(FishMoveInfo info) => info.IsPause = false;

        public static void SetMoveSpeed(FishMoveInfo info, float moveSpeed) => info.MoveSpeed = moveSpeed;
    }
}