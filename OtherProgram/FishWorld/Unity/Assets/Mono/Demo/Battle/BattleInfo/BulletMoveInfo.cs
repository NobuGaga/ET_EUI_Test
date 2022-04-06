using UnityEngine;

namespace ET
{
    /// <summary>
    /// 子弹移动组件数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 需要在热更层释放时将引用返回数据缓存池
    /// </summary>
	public class BulletMoveInfo
    {
        /// <summary> 当前帧旋转 </summary>
        public Quaternion CurrentRotation;

        /// <summary> 移动速度 </summary>
        public float MoveSpeed;

        /// <summary> 移动方向, 单位向量 </summary>
        public Vector2 MoveDirection;

        /// <summary> 当前子弹局部坐标 </summary>
        public Vector3 CurrentLocalPos;

        /// <summary> 追踪位置, 每帧更新 </summary>
        public Vector3 TrackPosition;
    }

    public static class BulletMoveDefaultInfo
    {
        /// <summary> 子弹默认追踪位置 </summary>
        public static Vector3 TrackPosition = new Vector3(-100, -100, -100);
    }
}