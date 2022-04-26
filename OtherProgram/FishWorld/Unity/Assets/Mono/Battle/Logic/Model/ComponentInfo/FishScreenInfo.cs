using UnityEngine;

namespace ET
{
    /// <summary> Mono 层鱼屏幕数据类 </summary>
	public class FishScreenInfo
    {
        /// <summary> 瞄准点, 用来定位瞄准技能特效跟, 撒网特效的点(屏幕坐标) </summary>
        public Vector3 AimPoint;

        /// <summary> 是否在屏幕内, 这个标记暂时只有鱼才有用, 所以放到这里 </summary>
        public bool IsInScreen;
    }
}