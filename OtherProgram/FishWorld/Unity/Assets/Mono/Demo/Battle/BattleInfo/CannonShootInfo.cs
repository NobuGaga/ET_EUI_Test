using UnityEngine;

namespace ET
{
    /// <summary>
    /// 炮台射击数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 
    /// 
    /// 
    /// 需要在热更层释放时将引用返回数据缓存池
    /// 
    /// 
    /// 
    /// </summary>
	public class CannonShootInfo : BattleBaseInfo
    {
        /// <summary> 炮台旋转四元数, 用于计算 ShootDirection </summary>
        public Quaternion LocalRotation;

        /// <summary> 炮台射击方向, 对应 BulletMoveInfo.MoveDirection </summary>
        public Vector2 ShootDirection;

        /// <summary> 炮台射击点屏幕坐标 </summary>
        public Vector3 ShootScreenPosition;

        /// <summary> 射击子弹初始位置 </summary>
        public Vector2 ShootLocalPosition;

        /// <summary>
        /// 初始化方法, 因为 MonoPool.Instance.Fetch 通过反射实例化类
        /// 所以不能使用带参数的构造函数, 不要实例化之后手动调用
        /// </summary>
        /// <param name="LocalRotation">炮台旋转四元数</param>
        /// <param name="ShootScreenPosition">炮台射击点屏幕坐标</param>
        public void Init(Quaternion LocalRotation, Vector3 ShootScreenPosition)
        {
            this.LocalRotation = LocalRotation;
            this.ShootScreenPosition = ShootScreenPosition;
        }
    }
}