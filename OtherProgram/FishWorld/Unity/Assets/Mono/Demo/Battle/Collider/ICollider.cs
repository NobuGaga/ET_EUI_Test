using UnityEngine;

namespace ET
{
    /// <summary> 碰撞体接口 </summary>
    public interface ICollider
    {
        /// <summary>
        /// 根据骨骼点世界坐标跟模型缩放值更新碰撞结构体
        /// 不同结构体更新逻辑跟时序不一样
        /// 在这个更新接口里统一处理
        /// 特殊的碰撞体则在自己的类里面实现别的 Update 接口
        /// </summary>
        /// <param name="boneWorldPoint">骨骼点世界坐标</param>
        /// <param name="scale">模型缩放</param>
        void Update(ref Vector3 boneWorldPoint, float scale);
    }

    public static class IColliderExtension
    {
        public static bool IsCollide(this ICollider self, ICollider other)
        {
            if (self is Point3D)
            {
                Point3D selfPoint = (Point3D)self;
                if (other is Point3D)
                {
                    Point3D otherPoint = (Point3D)other;
                    return selfPoint.IsCollide(ref otherPoint, 0);
                }

                if (other is Sphere)
                {
                    Sphere sphere = (Sphere)other;
                    return selfPoint.IsCollide(ref sphere);
                }

                if (other is Line2D)
                {
                    Line2D line = (Line2D)other;
                    return selfPoint.IsCollide(ref line);
                }
            }

            if (self is Sphere)
            {
                Sphere selfSphere = (Sphere)self;
                if (other is Point3D)
                {
                    Point3D otherPoint = (Point3D)other;
                    return selfSphere.IsCollide(ref otherPoint);
                }

                if (other is Sphere)
                {
                    Sphere sphere = (Sphere)other;
                    return selfSphere.IsCollide(ref sphere);
                }

                if (other is Line2D)
                {
                    Line2D line = (Line2D)other;
                    return selfSphere.IsCollide(ref line);
                }
            }

            if (self is Line2D)
            {
                Line2D selfLine = (Line2D)self;
                if (other is Point3D)
                {
                    Point3D otherPoint = (Point3D)other;
                    return selfLine.IsCollide(ref otherPoint);
                }

                if (other is Sphere)
                {
                    Sphere sphere = (Sphere)other;
                    return selfLine.IsCollide(ref sphere);
                }
            }

            return false;
        }
    }
}