// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动组件数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 需要在热更层释放时将引用返回数据缓存池
    /// </summary>
	public class FishMoveInfo
    {
        /// <summary> 暂停标识 </summary>
        public bool IsPause;

        /// <summary> 移动结束标识(到达生命周期末端, 存活结束时间点) </summary>
        public bool IsMoveEnd;

        /// <summary> 鱼线总运动时间, 毫秒 </summary>
        public uint MoveDuration;

        /// <summary>
        /// 鱼线当前已运动时间, 毫秒, 为负数则还没到出生时间, 通过累加时间到达正数开始移动
        /// </summary>
        public int MoveTime;

        /// <summary> 当前生命周期, 已运动时间除以总运动时间 </summary>
        public float CurrentLifeTime;

        // 三轴偏移值, 每个鱼的偏移值都不一样, 不跟随鱼线变化(跟鱼变化)
        public float OffsetPosX;
        public float OffsetPosY;
        public float OffsetPosZ;

        /// <summary> 下一帧位置 </summary>
        public Vector3 NextPos;

        /// <summary> 下一帧朝向 </summary>
        public Vector3 NextForward;

        /// <summary> 移动速度 </summary>
        public float MoveSpeed;

        /// <summary> 鱼线表 ID, 放在这里储存用来获取对应的 FishPath 类 </summary>
        public short RoadId;

        /// <summary> 是否通过鱼线控制移动 </summary>
        public bool IsPathMove;

        public FishPathInfo Path;

        /// <summary> 是否到达生命周期时间 </summary>
        public bool IsMoveTimeOut => MoveTime > MoveDuration;

        /// <summary> 设置停止移动标识 </summary>
        public void StopMove()
        {
            IsPause = false;
            IsMoveEnd = true;
        }
    }
}