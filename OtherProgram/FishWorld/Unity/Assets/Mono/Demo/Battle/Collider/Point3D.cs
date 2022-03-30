using UnityEngine;

namespace ET
{
    /// <summary> 战斗碰撞结构体(点) </summary>
    public struct Point3D : ICollider
    {
        /// <summary> 摄像机类型(固定值) </summary>
        private ushort cameraType;
        public ushort CameraType => cameraType;

        /// <summary> 骨骼点世界坐标(根据上一帧骨骼点更新) </summary>
        private Vector3 center;
        public Vector3 Center => center;

        /// <summary> 距离模型原点的偏移量(固定值)(相对世界坐标) </summary>
        private Vector3 offset;

        /// <summary> 模型缩放值(根据上一帧骨骼点更新) </summary>
        public float scale;

        /// <summary> </summary>
        /// <param cameraType="摄像机类型"></param>
        /// <param offset="世界坐标偏移值"></param>
        public Point3D(ushort cameraType, Vector3 offset)
        {
            this.cameraType = cameraType;
            center = offset;
            this.offset = offset;
            scale = ColliderConfig.DefaultScale;
        }

        #region ICollider

        public void Update(ref Vector3 boneWorldPoint, float scale)
        {
            this.scale = scale;
            Vector3 boneScreenPos = ColliderHelper.GetScreenPoint(cameraType, ref boneWorldPoint);
            UpdateWithScreenPosition(ref boneScreenPos, scale);
        }

        #endregion

        public void UpdateWithScreenPosition(ref Vector3 boneScreenPos, float scale)
        {
            this.scale = scale;
            center.Set(offset.x * this.scale + boneScreenPos.x, offset.y * this.scale + boneScreenPos.y,
                       offset.z * this.scale + boneScreenPos.z);
        }

        public bool IsCollide(ref Point3D point, float distance) =>
            DistanceX(ref point) < distance && DistanceY(ref point) < distance;

        private float DistanceX(ref Point3D point) => Mathf.Abs(center.x - point.center.x);

        private float DistanceY(ref Point3D point) => Mathf.Abs(center.y - point.center.y);
    }
}