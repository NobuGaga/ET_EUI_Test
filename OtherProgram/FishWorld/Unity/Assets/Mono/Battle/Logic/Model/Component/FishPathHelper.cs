using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> 鱼线路径 Path 助手, 避免反复创建相同的Path </summary>
    public static class FishPathHelper
    {
        /// <summary> 鱼线配置表配置项数量最值 </summary>
        internal const ushort DefaultPathCount = 128;

        /// <summary> 鱼线配置表点的个数最值 </summary>
        private const ushort DefaultPathPointCount = 64;

        /// <summary> 首尾补齐点长度(世界坐标) </summary>
        private const ushort ExpandPosLength = 10;

        private static readonly Dictionary<short, FishPathInfo> _pathMap;

        private static readonly List<Vector3> _pathPointCache;

        static FishPathHelper()
        {
            _pathMap = new Dictionary<short, FishPathInfo>(DefaultPathCount);
            _pathPointCache = new List<Vector3>(DefaultPathPointCount);
        }

        public static void Clear()
        {
            _pathMap.Clear();
            _pathPointCache.Clear();
        }

        public static FishPathInfo GetPath(short roadId)
        {
            if (_pathMap.ContainsKey(roadId))
                return _pathMap[roadId];
            return CreatePath(roadId);
        }

        private static FishPathInfo CreatePath(short roadId)
        {
            ReadFishLineRela(roadId);
            ExpandPosList();
            Vector3[] posList = _pathPointCache.ToArray();
            FishPathInfo path = new FishPathInfo(posList);
            _pathMap.Add(roadId, path);
#if UNITY_EDITOR

            BattleDebugComponent.OpenPathDebug(roadId, posList);
#endif
            return path;
        }

        private static Func<short, string[]> getFishLineRelaList;
        public static void SetGetFishLineRelaFunc(Func<short, string[]> func) => getFishLineRelaList = func;

        private static void ReadFishLineRela(short roadId)
        {
            _pathPointCache.Clear();
            // Battle Warning 使用委托可能存在一定问题
            if (getFishLineRelaList == null)
            {
                Log.Error("Model ET.FishLineConfigCategory not call FishPathHelper.SetGetFishLineRelaListFunc");
                //Debug.LogError("Model ET.FishLineConfigCategory not call FishPathHelper.SetGetFishLineRelaListFunc");
                return;
            }
            string[] configLineRela = getFishLineRelaList(roadId);
            VectorStringHelper.TryParseVectorStringArray(configLineRela, _pathPointCache);
        }

        private static Vector3 GetExpandPos(Vector3 startPos, Vector3 endPos) =>
                        Vector3.Normalize(endPos - startPos) * ExpandPosLength + endPos;

        // TODO 这里是按照直线进行的延长的. 根据分辨率计算需要加多少, 和相机的 fov 值也会有关系
        // 根据分辨率, 鱼线进入, 出去的方式决定是否增加一部分头尾的距离
        public static void ExpandPosList()
        {
            if (_pathPointCache.Count < 2)
            {
                //Debug.LogError("FishPathManager ExpandPosList fish line point must at lest 1");
                return;
            }
            Vector3 firstPathPos = _pathPointCache[1];
            Vector3 secondPathPos = _pathPointCache[2];
            _pathPointCache.Insert(0 , GetExpandPos(secondPathPos, firstPathPos));
            Vector3 lastPathPos = _pathPointCache[_pathPointCache.Count - 1];
            Vector3 lastSecondPathPos = _pathPointCache[_pathPointCache.Count - 2];
            _pathPointCache.Add(GetExpandPos(lastSecondPathPos, lastPathPos));
        }
    }
}