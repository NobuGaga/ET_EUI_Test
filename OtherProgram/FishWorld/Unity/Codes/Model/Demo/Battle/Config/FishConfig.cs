using UnityEngine;

namespace ET
{
	public static class FishConfig
	{
        /// <summary> 毫秒转换 </summary>
        public const ushort MilliSecond = 1000;

        /// <summary> 服务器传偏移值修正比例 </summary>
        public const ushort ServerOffsetScale = 100;

        /// <summary>
        /// 默认移动速度, 这里的速度都是通过乘以时间间隔实现的
        /// 实际是一个更新时间步长修正值
        /// </summary>
        public const ushort DefaultMoveSpeed = 1;

        /// <summary> 驱赶鱼移动速度 </summary>
        public const ushort QuickMoveSpeed = 30;

        // Battle Warning 成员变量值类型 GC 标记
        /// <summary> 鱼移除屏幕坐标点 </summary>
        public static Vector3 RemovePoint = new Vector3(-10000, 0, 0);

        /// <summary> 鱼回收节点名, 只在编辑器模式下设置 </summary>
        public const string DefaultName = "fish_die_node";

        /// <summary> 鱼存活节点名, 只在编辑器模式下设置 </summary>
        public const string NameFormat = "fish_{0}_{1}";

        /// <summary> Asset Bundle Path 路径格式 </summary>
        public const string FishAssetBundlePathFormat = "bundles/prefabs/fishes/fish_{0}.unity3d";

        /// <summary> Asset Name 资源格式 </summary>
        public const string FishAssetNameFormat = "fish_{0}";

        /// <summary> Asset Clip Bundle Path 路径格式 </summary>
        public const string FishAssetClipBundlePathFormat = "bundles/clip/fishes/fish_{0}.unity3d";
    }
}