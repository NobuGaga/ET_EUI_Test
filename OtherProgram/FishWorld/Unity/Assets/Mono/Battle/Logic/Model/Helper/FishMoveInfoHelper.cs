// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用
    /// 用于处理复杂的鱼线计算
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class FishMoveInfoHelper
    {
        public static FishMoveInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.FishMoveInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(FishMoveInfo)) as FishMoveInfo;
            unit.FishMoveInfo = info;
            return info;
        }

        /// <summary> 使用服务器数据初始化移动数据 </summary>
        /// <param name="info">移动数据</param>
        /// <param name="roadId">鱼线表数据</param>
        /// <param name="totalMoveTime">总移动时间(毫秒)</param>
        /// <param name="offsetPosX">初始位置偏移值 X</param>
        /// <param name="offsetPosY">初始位置偏移值 Y</param>
        /// <param name="offsetPosZ">初始位置偏移值 Z</param>
        /// <param name="configSpeed">配置表初始速度值</param>
        /// <param name="isPause">当前时刻是否暂停</param>
        public static void InitInfo(FishMoveInfo info, short roadId, int totalMoveTime,
                                                       float offsetPosX, float offsetPosY, float offsetPosZ,
                                                       int configSpeed)
        {
            info.MoveDuration = totalMoveTime;
            if (info.IsMoveTimeOut)
            {
                info.StopMove();
                return;
            }

            info.IsPause = false;
            info.IsMoveEnd = false;

            info.RoadId = roadId;
            FishPathInfo path = info.RoadId > 0 ? FishPathHelper.GetPath(info.RoadId) : null;
            info.Path = path;
            info.IsPathMove = path != null;

            info.OffsetPosX = offsetPosX;
            info.OffsetPosY = offsetPosY;
            info.OffsetPosZ = offsetPosZ;
            info.OriginConfigSpeed = configSpeed;
            info.CurrentConfigSpeed = configSpeed;

            if (info.MoveTime < 0 || !info.IsPathMove)
                return;

            info.CurrentLifeTime = (float)info.MoveTime / info.MoveDuration;
            CheckNextTimeValid(ref info.CurrentLifeTime);
            float nextTime = info.CurrentLifeTime;
            SetForward(info, ref nextTime);

            nextTime = (float)(info.MoveTime + TimeHelper.ClinetDeltaFrameTime()) / info.MoveDuration;
            CheckNextTimeValid(ref nextTime);
            SetNextPosition(info, ref nextTime);
        }

        public static void FixedUpdate(FishMoveInfo info)
        {
            if (info.IsPause || info.IsMoveEnd) return;

            if (info.IsMoveTimeOut)
            {
                info.StopMove();
                return;
            }

            float deltaTime = TimeHelper.ClinetDeltaFrameTime();

            // 默认加速度为 0, 先计算加速后的速度值, 再获取指定
            info.MoveSpeed += deltaTime * info.MoveAcceleration;
            info.MoveTime += (int)(info.MoveSpeed * deltaTime);
            if (info.MoveTime < 0) return;

            if (!info.IsPathMove) return;

            info.CurrentLifeTime = (float)info.MoveTime / info.MoveDuration;
            CheckNextTimeValid(ref info.CurrentLifeTime);
            float nextTime = info.CurrentLifeTime;
            SetForward(info, ref nextTime);
            SetNextPosition(info, ref nextTime);
        }

        private static void SetForward(FishMoveInfo info, ref float percentFloat)
        {
            ref Vector3 nextPos = ref info.NextPos;
            info.Path.SetForward(ref percentFloat, nextPos.x - info.OffsetPosX, nextPos.y - info.OffsetPosY,
                                                   nextPos.z - info.OffsetPosZ, ref info.NextForward);
        }

        private static void SetNextPosition(FishMoveInfo info, ref float percentFloat)
        {
            ref Vector3 nextPos = ref info.NextPos;
            info.Path.SetPoint(ref percentFloat, ref nextPos);
            nextPos.x += info.OffsetPosX;
            nextPos.y += info.OffsetPosY;
            nextPos.z += info.OffsetPosZ;
        }

        private static void CheckNextTimeValid(ref float nextTime) { if (nextTime > 1) nextTime = 1; }
    }
}