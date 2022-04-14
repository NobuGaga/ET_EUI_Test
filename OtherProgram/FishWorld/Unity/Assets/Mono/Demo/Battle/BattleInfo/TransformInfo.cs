using UnityEngine;

namespace ET
{
    /// <summary> Mono 层变换组件数据类 </summary>
	public class TransformInfo
    {
        public Vector3 LogicPos;
        public Vector3 LogicLocalPos;
        public Quaternion LogicRotation;
        public Quaternion LogicLocalRotation;
        public Vector3 LogicScale;
        public Vector3 LogicForward;

        /// <summary>
        /// 屏幕坐标, 放在 Model 层, 但是计算要在 HotfixView 层进行(因为要用到 Camera)
        /// 因此赋值在 HotfixView 层进行, 但逻辑执行还是在 Hotfix 里
        /// </summary>
        public Vector3 ScreenPos;
    }
}