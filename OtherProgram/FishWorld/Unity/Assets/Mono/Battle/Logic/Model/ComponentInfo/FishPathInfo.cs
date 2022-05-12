// Battle Review Before Boss Node

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> 模拟 DoTween FishPath </summary>
    public class FishPathInfo
    {
        /// <summary> 缓存 key 精度值, 数值越大越精确 </summary>
        private const ushort FixPercentInteger = 10000;

        /// <summary> 根据时间变换产生的鱼线点的个数, 会很大 </summary>
        private const ushort DefaultPointCacheCount = 1024;

        /// <summary> 防止朝向为 0 误差值判断 </summary>
        private const float FixRotationFlag = 0.0001f;

        private static Vector3 cachePoint;

        /// <summary> 根据时间 delta 变化储存的坐标点 </summary>
        private Dictionary<ushort, Vector3> timePointMap;

        /// <summary> 鱼线路径点 </summary>
        private Vector3[] pointArray;

        private Vector3 secondPoint;

        /// <summary>
        /// 最后延长点
        /// 跑完鱼线后再跑一遍鱼线(除了不跑最后一个点外)
        /// 为了增加移除屏幕距离
        /// </summary>
        private Vector3 extensionLastPoint;

        public FishPathInfo(Vector3[] pointArray)
        {
            timePointMap = new Dictionary<ushort, Vector3>(DefaultPointCacheCount);
            this.pointArray = pointArray;
            secondPoint = pointArray[1];
            int length = pointArray.Length;
            extensionLastPoint = pointArray[length - 1] + pointArray[length - 2];
        }

        public void SetForward(ref float percentFloat, float nextPosX, float nextPosY, float nextPosZ, ref Vector3 toForward)
        {
            SetPoint(ref percentFloat, ref cachePoint);
            float x = cachePoint.x - nextPosX;
            float y = cachePoint.y - nextPosY;
            float z = cachePoint.z - nextPosZ;

            // 防止设置朝向为 0
            if (Math.Abs(z) < FixRotationFlag)
                z = z > 0 ? FixRotationFlag : -FixRotationFlag;

            toForward.x = x;
            toForward.y = y;
            toForward.z = z;
        }

        /// <summary> 根据时间设置路径点 </summary>
        public void SetPoint(ref float percentFloat, ref Vector3 point)
        {
            if (pointArray.Length < 2)
            {
                point = Vector3.zero;
                return;
            }

            ushort percentInt = (ushort)Math.Floor(percentFloat * FixPercentInteger);
            if (timePointMap.ContainsKey(percentInt))
            {
                point = timePointMap[percentInt];
                return;
            }

            int sectionLength = pointArray.Length - 2;
            ushort section = (ushort)Math.Floor(percentFloat * sectionLength);
            int currentLength = sectionLength - 1;
            if (currentLength > section) currentLength = section;

            float u = percentFloat * sectionLength - currentLength;

            Vector3 a = currentLength == 0 ? secondPoint : pointArray[currentLength];
            Vector3 b = pointArray[currentLength + 1];
            Vector3 c = pointArray[currentLength + 2];
            Vector3 d = currentLength + 3 >= pointArray.Length ? extensionLastPoint : pointArray[currentLength + 3];

            float u2 = u * u;
            float u3 = u2 * u;

            point.x = CalculateCatmulRomPointSingle(u, u2, u3, a.x, b.x, c.x, d.x);
            point.y = CalculateCatmulRomPointSingle(u, u2, u3, a.y, b.y, c.y, d.y);
            point.z = CalculateCatmulRomPointSingle(u, u2, u3, a.z, b.z, c.z, d.z);

            timePointMap.Add(percentInt, point);
        }

        private static float CalculateCatmulRomPointSingle(float u, float u2, float u3, float a, float b, float c, float d)
        {
            float term1 = (-a + b * 3 - c * 3 + d) * u3;
            float term2 = (a * 2 - b * 5 + c * 4 - d) * u2;
            float term3 = (-a + c) * u + b * 2;
            return (term1 + term2 + term3) * 0.5f;
        }
    }
}