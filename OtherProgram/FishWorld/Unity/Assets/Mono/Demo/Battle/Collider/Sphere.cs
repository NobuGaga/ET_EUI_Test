using System;
using UnityEngine;

namespace ET
{
    /// <summary> 战斗碰撞结构体(球) </summary>
    public struct Sphere : ICollider
    {
        public Point3D CenterPoint;

        public Vector3 Center => CenterPoint.Center;

        /// <summary> 球半径 </summary>
        private float radius;
        /// <summary> 屏幕坐标半径值 </summary>
        public float Radius => radius * CenterPoint.scale;

        /// <summary> </summary>
        /// <param cameraType="摄像机类型"></param>
        /// <param offset="世界坐标偏移值"></param>
        /// <param radius="球半径"></param>
        public Sphere(ushort cameraType, Vector3 offset, float radius)
        {
            CenterPoint = new Point3D(cameraType, offset);
            this.radius = radius;
        }

        public bool IsCollide(ref Sphere sphere) =>
                    CenterPoint.IsCollide(ref sphere.CenterPoint, Radius + sphere.Radius);

        #region ICollider

        public void Update(ref Vector3 boneWorldPoint, float scale)
        {
            Vector3 screenCenterPos = ColliderHelper.GetScreenPoint(CenterPoint.CameraType, ref boneWorldPoint);
            boneWorldPoint.x += radius;
            Vector3 screenBorderPos = ColliderHelper.GetScreenPoint(CenterPoint.CameraType, ref boneWorldPoint);

            float distanceX = screenCenterPos.x - screenBorderPos.x;
            float distanceY = screenCenterPos.y - screenBorderPos.y;

            scale = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY) / radius;
            CenterPoint.UpdateWithScreenPosition(ref screenCenterPos, scale);
        }

        #endregion
    }

    public static class SphereExtension
    {
        public static bool IsCollide(this ref Sphere sphere, ref Point3D point) =>
                            sphere.CenterPoint.IsCollide(ref point, sphere.Radius);

        public static bool IsCollide(this ref Point3D point, ref Sphere sphere) =>
                            sphere.IsCollide(ref point);
    }
}