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

        public void Update(FishMoveInfo info)
        {
            LogicLocalPos = info.NextPos;
            LogicForward = info.NextForward;
        }

        public void Update(BulletMoveInfo info)
        {
            LogicLocalPos = info.CurrentLocalPos;
            LogicLocalRotation = info.CurrentRotation;
        }
    }
}