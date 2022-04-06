using UnityEngine;

namespace ET
{
	public static class BulletConfig
    {
        // Battle Warning 成员变量值类型 GC 标记

        /// <summary> 默认子弹 Unit ID </summary>
        public const ushort DefaultBulletUnitId = 0;

        /// <summary> 子弹的 Unit Config ID </summary>
        public const ushort BulletUnitConfigId = 0;

        /// <summary> 默认子弹追踪的鱼 Unit ID </summary>
        public const ushort DefaultTrackFishUnitId = 0;

        /// <summary> 触控屏幕坐标转换修正值 </summary>
        public const float TouchScreenPosFix = 100;

        // Battle TODO delete
        /// <summary> 子弹碰撞体配置 ID </summary>
        public const int BulletColliderID = 1;

        /// <summary>
        /// 默认移动速度, 这里的速度都是通过乘以时间间隔实现的
        /// 实际是一个更新时间步长修正值
        /// </summary>
        public const float DefaultMoveSpeed = 0.6f;

        /// <summary> 默认移动方向, 屏幕坐标向上 </summary>
        public static Vector2 DefaultMoveDirection = new Vector2(0, 1);

        /// <summary> 子弹移除屏幕坐标点 </summary>
        public static Vector3 RemovePoint = new Vector3(10000, 0, 0);

        /// <summary> 单个玩家发射子弹上限 先写死方便生成 ID 后面改成读表 </summary>
        public const ushort ShootMaxBulletCount = 30;

        /// <summary> 射击子弹时间间隔毫秒 </summary>
        public const ushort ShootBulletInterval = 200;

        /// <summary>
        /// 子弹 ID 校正值, 该值等于个人发射子弹上限值数字位数加多一位
        /// </summary>
        public static int BulletIdFix;

        /// <summary> 子弹回收节点名, 只在编辑器模式下设置 </summary>
        public const string DefaultName = "bullet_die_node";

        /// <summary> 子弹存活节点名, 只在编辑器模式下设置 </summary>
        public const string NameFormat = "bullet_{0}";

        // 子弹 ID 生成校正值, 后面放到全局表的 partial Category AfterEndInit() 里实现
        static BulletConfig()
        {
            // 计算自己发射子弹最大值是几位数, 默认个位数
            ushort count = 1;
            ushort maxBulletCount = ShootMaxBulletCount;

            while (maxBulletCount > 10)
            {
                maxBulletCount /= 10;
                count++;
            }

            BulletIdFix = (int)Mathf.Pow(10, count);
        }
    }
}