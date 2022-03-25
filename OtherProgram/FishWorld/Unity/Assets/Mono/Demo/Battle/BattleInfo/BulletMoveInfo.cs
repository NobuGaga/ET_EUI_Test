using UnityEngine;

namespace ET
{
    /// <summary>
    /// 子弹移动组件数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 需要在热更层释放时将引用返回数据缓存池
    /// </summary>
	public class BulletMoveInfo : BattleBaseInfo
    {
        /// <summary> 下一帧旋转 </summary>
        public Quaternion NextRotation;

        /// <summary> 移动速度 </summary>
        public float MoveSpeed;

        /// <summary> 移动方向, 单位向量 </summary>
        public Vector2 MoveDirection;

        /// <summary> 下一帧位移 </summary>
        public Vector2 NextStep;
    }
}