using UnityEngine;

namespace ET
{
	public static class FishConfig
	{
        /// <summary> 服务器传偏移值修正比例 </summary>
        public const ushort ServerOffsetScale = 100;

        /// <summary> 毫秒转换 </summary>
        public const ushort MilliSecond = 1000;

        /// <summary>
        /// 默认移动速度, 这里的速度都是通过乘以时间间隔实现的
        /// 实际是一个更新时间步长修正值
        /// </summary>
        public const ushort DefaultMoveSpeed = 1;

        /// <summary> 鱼移除屏幕坐标点 </summary>
        public static Vector3 RemovePoint = new Vector3(0, 0, -10000);

        /// <summary> 鱼回收节点名, 只在编辑器模式下设置 </summary>
        public const string DefaultName = "fish_die_node";

        /// <summary> 鱼存活节点名, 只在编辑器模式下设置 </summary>
        public const string NameFormat = "fish_{0}_{1}";
    }
}