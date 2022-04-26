using UnityEngine;

namespace ET
{
    /// <summary> Mono 层变换组件数据类 </summary>
	public class TransformInfo
    {
        public Vector3 WorldPosition;
        public Vector3 LocalPosition;

        public Quaternion Rotation;
        public Quaternion LocalRotation;
        
        public Vector3 Forward;

        public void Update(FishMoveInfo info)
        {
            LocalPosition = info.NextPos;
            Forward = info.NextForward;
        }

        public void Update(BulletMoveInfo info)
        {
            LocalPosition = info.CurrentLocalPos;
            LocalRotation = info.CurrentRotation;
        }
    }
}