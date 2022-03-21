using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    // 模拟 DoTween FishPath
    public class FishPathInfo : BattleBaseInfo
    {
        // 缓存 key, 使用整数, 数值越大越精确
        private const ushort PERC_INT_RESOLUTION = 10000;
        // 根据时间变换产生的鱼线点的个数, 会很大
        private const ushort DefaultPointCacheCount = 1024;

        // 防止朝向为 0 误差值判断
        private const float FixRotationFlag = 0.0001f;
        private static Vector3 _cachePoint;

        // 缓存鱼线实际根据时间变化产生的坐标点
        private Dictionary<ushort, Vector3> _percentPosMap;

        // 缓存鱼线路径点
        private Vector3[] _posList;

        // 第二个点
        private Vector3 _secondPos;
        // TODO 最后两个点相加构成的点, 为了究极延长?
        private Vector3 _lastTwoPointSum;

        public FishPathInfo(Vector3[] posList)
        {
            _percentPosMap = new Dictionary<ushort, Vector3>(DefaultPointCacheCount);
            _posList = posList;
            _secondPos = posList[1];
            int wpsLen = posList.Length;
            _lastTwoPointSum = posList[wpsLen - 1] + posList[wpsLen - 2];
        }

        ~FishPathInfo()
        {
            _percentPosMap.Clear();
            _percentPosMap = null;
            _posList = null;
        }

        // 根据时间获取路径点
        public void SetPoint(float percentFloat, ref Vector3 point)
        {
            ushort percentInt = (ushort)System.Math.Floor(percentFloat * PERC_INT_RESOLUTION);
            if (_percentPosMap.ContainsKey(percentInt))
            {
                point = _percentPosMap[percentInt];
                return;
            }

            if (_posList.Length < 2)
            {
                point = Vector3.zero;
                return;
            }

            int numSections = _posList.Length - 2;
            ushort tSec = (ushort)System.Math.Floor(percentFloat * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec)
                currPt = tSec;

            float u = percentFloat * numSections - currPt;

            Vector3 a = currPt == 0 ? _secondPos : _posList[currPt];
            Vector3 b = _posList[currPt + 1];
            Vector3 c = _posList[currPt + 2];
            Vector3 d = currPt + 3 >= _posList.Length ? _lastTwoPointSum : _posList[currPt + 3];

            float u2 = u * u;
            float u3 = u2 * u;

            float x = CalcCatmulRomPointSingle(u, u2, u3, a.x, b.x, c.x, d.x);
            float y = CalcCatmulRomPointSingle(u, u2, u3, a.y, b.y, c.y, d.y);
            float z = CalcCatmulRomPointSingle(u, u2, u3, a.z, b.z, c.z, d.z);

            point.x = x;
            point.y = y;
            point.z = z;
            //point.Set(x, y, z);

            _percentPosMap.Add(percentInt, point);
        }

        public void SetForward(float percentFloat, float nextPosX, float nextPosY, float nextPosZ, ref Vector3 toForward)
        {
            SetPoint(percentFloat, ref _cachePoint);
            float x = _cachePoint.x - nextPosX;
            float y = _cachePoint.y - nextPosY;
            float z = _cachePoint.z - nextPosZ;
            // 防止设置朝向为 0
            if (System.Math.Abs(z) < FixRotationFlag)
                z = z > 0 ? FixRotationFlag : -FixRotationFlag;
            toForward.x = x;
            toForward.y = y;
            toForward.z = z;
            //toForward.Set(x, y, z);
        }

        private static float CalcCatmulRomPointSingle(float u, float u2, float u3, float a, float b, float c, float d)
        {
            float term1 = (-a + b * 3 - c * 3 + d) * u3;
            float term2 = (a * 2 - b * 5 + c * 4 - d) * u2;
            float term3 = (-a + c) * u + b * 2;
            return (term1 + term2 + term3) * 0.5f;
        }
    }
}