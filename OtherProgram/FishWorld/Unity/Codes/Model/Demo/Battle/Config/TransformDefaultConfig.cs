using UnityEngine;

namespace ET
{
    /// <summary>
    /// 默认 Transform 初始值配置
    /// </summary>
	public static class TransformDefaultConfig
	{
        public static Vector3 DefaultPosition = Vector3.zero;
        public static Quaternion DefaultRotation = Quaternion.identity;
        public static Vector3 DefaultScale = new Vector3(0.1f, 0.1f, 0.1f);
        public static Vector3 DefaultForward = Vector3.forward;
        
        /// <summary> 默认屏幕坐标, 设置一个不会参与碰撞的位置 </summary>
        public static Vector3 DefaultScreenPos = new Vector3(-100, -100, -100);
        
        public const string DefaultName = "DefaultName";
    }
}