using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动辅助类, 对应 ILRuntime 层 FishMoveComponent 调用方法
    /// 用于处理复杂的计算问题
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class FishMoveHelper
    {
        public static FishMoveInfo PopInfo() =>
                                MonoPool.Instance.Fetch(typeof(FishMoveInfo)) as FishMoveInfo;

        public static void PushPool(FishMoveInfo info) => MonoPool.Instance.Recycle(info);

        public static bool IsMoveTimeOut(this FishMoveInfo info) => info.MoveTime > info.MoveDuration;

        private static FishPathInfo Path(this FishMoveInfo info) =>
                                        info.RoadId > 0 ? FishPathHelper.GetPath(info.RoadId) : null;

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

            if (info.IsMoveTimeOut())
            {
                StopMove(info);
                return;
            }

            info.RoadId = roadId;
            FishPathInfo path = info.Path();
            info.IsPathMove = path != null;

            info.OffsetPosX = offsetPosX;
            info.OffsetPosY = offsetPosY;
            info.OffsetPosZ = offsetPosZ;

            if (info.MoveTime < 0)
                return;

            float nextTime = (float)info.MoveTime / info.MoveDuration;
            info.SetForward(path, nextTime);

            nextTime = (float)(info.MoveTime + TimeHelper.ClinetDeltaFrameTime()) / info.MoveDuration;
            info.SetNextPosition(path, nextTime);
        }

        public static void FixedUpdate(FishMoveInfo info)
        {
            if (info.IsPause || info.IsMoveEnd)
                return;

            if (info.IsMoveTimeOut())
            {
                StopMove(info);
                return;
            }

            info.MoveTime += (int)(info.MoveSpeed * TimeHelper.ClinetDeltaFrameTime());
            if (info.MoveTime < 0)
                return;

            if (info.IsPathMove)
                info.UpdateNextPosAndForwardByPath();
        }

        private static void UpdateNextPosAndForwardByPath(this FishMoveInfo info)
        {
            FishPathInfo path = info.Path();
            float nextTime = (float)info.MoveTime / info.MoveDuration;
            info.SetForward(path, nextTime);
            info.SetNextPosition(path, nextTime);
        }

        private static void SetForward(this FishMoveInfo info, FishPathInfo path, float percentFloat)
        {
            if (!info.IsPathMove)
                return;

            CheckNextTimeValid(ref percentFloat);
            ref Vector3 nextPos = ref info.NextPos;
            path.SetForward(percentFloat, nextPos.x - info.OffsetPosX, nextPos.y - info.OffsetPosY,
                                            nextPos.z - info.OffsetPosZ, ref info.NextForward);
        }

        private static void SetNextPosition(this FishMoveInfo info, FishPathInfo path, float percentFloat)
        {
            if (!info.IsPathMove)
                return;

            CheckNextTimeValid(ref percentFloat);
            ref Vector3 nextPos = ref info.NextPos;
            path.SetPoint(percentFloat, ref nextPos);
            nextPos.x += info.OffsetPosX;
            nextPos.y += info.OffsetPosY;
            nextPos.z += info.OffsetPosZ;
        }

        private static void CheckNextTimeValid(ref float nextTime)
        {
            if (nextTime > 1)
                nextTime = 1;
        }

        public static void SetMoveSpeed(FishMoveInfo info, float moveSpeed) => info.MoveSpeed = moveSpeed;

        public static void PauseMove(FishMoveInfo info) => info.IsPause = true;

        public static void ResumeMove(FishMoveInfo info) => info.IsPause = false;

        public static void StopMove(FishMoveInfo info)
        {
            info.IsPause = false;
            info.IsMoveEnd = true;
        }
    }
}