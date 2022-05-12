using UnityEngine;

namespace ET
{
    /// <summary>
    /// 炮台射击数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 需要在热更层释放时将引用返回数据缓存池
    /// </summary>
	public class CannonShootInfo
    {
        /// <summary> 炮台旋转四元数, 用于计算 ShootDirection </summary>
        public Quaternion LocalRotation;

        /// <summary> 炮台射击方向, 对应 BulletMoveInfo.MoveDirection, 单位向量 </summary>
        public Vector2 ShootDirection;

        /// <summary> 炮台射击点屏幕坐标 </summary>
        public Vector3 ShootPointScreenPos;

        /// <summary> 射击子弹初始位置 </summary>
        public Vector2 ShootLocalPosition;

        public Vector3 GetBulletStartPosition() =>
                       new Vector3(ShootLocalPosition.x, ShootLocalPosition.y, 0);
    }
}