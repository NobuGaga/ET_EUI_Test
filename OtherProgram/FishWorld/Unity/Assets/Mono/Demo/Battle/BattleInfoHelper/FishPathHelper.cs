using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{

    // Path 管理器, 避免反复创建相同的Path
    public static class FishPathHelper
    {

        // 鱼线配置表配置项数量最值
        private const ushort DefaultPathCount = 128;
        // 鱼线配置表点的个数最值
        private const ushort DefaultPathPointCount = 64;

        // 首尾补齐点长度
        private const ushort ExpandPosLength = 10;

        private static readonly Dictionary<Int16, FishPathInfo> _pathMap;
//#if UNITY_EDITOR

//        private static Dictionary<Int16, PathDebug> _debugMap;
//#endif
        private static readonly List<Vector3> _pathPointCache;

        static FishPathHelper()
        {
            _pathMap = new Dictionary<Int16, FishPathInfo>(DefaultPathCount);
//#if UNITY_EDITOR

//            _debugMap = new Dictionary<Int16, PathDebug>(DefaultPathCount);
//#endif
            _pathPointCache = new List<Vector3>(DefaultPathPointCount);
        }

        public static void Clear()
        {
            _pathMap.Clear();
//#if UNITY_EDITOR
//            _debugMap.Clear();
//#endif
            _pathPointCache.Clear();
        }

        public static FishPathInfo GetPath(Int16 roadId)
        {
            if (_pathMap.ContainsKey(roadId))
                return _pathMap[roadId];
            return CreatePath(roadId);
        }

        private static FishPathInfo CreatePath(Int16 roadId)
        {
            ReadFishLineRela(roadId);
            ExpandPosList();
            Vector3[] posList = _pathPointCache.ToArray();
            FishPathInfo path = new FishPathInfo(posList);
            _pathMap.Add(roadId, path);
//#if UNITY_EDITOR
//            OpenPathDebug(roadId, posList);
//#endif
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
            VectorStringHelper.TryParseVector3StringArray(configLineRela, _pathPointCache);
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

//#if UNITY_EDITOR

//        // 开启鱼线调试, 使用 Gizmos 画线
//        private static void OpenPathDebug(Int16 roadId, Vector3[] posList)
//        {
//            PathDebug pathDebug;
//            if (_debugMap.ContainsKey(roadId))
//            {
//                pathDebug = _debugMap[roadId];
//                pathDebug.ClearList();
//            }
//            else
//            {
//                GameObject go = new GameObject();
//                go.name = string.Format("PathDebug_{0}", roadId);
//                pathDebug = go.AddComponent<PathDebug>();
//                go.transform.SetParent(ReferenceHelper.FishRootNode.transform);
//                go.transform.SetSiblingIndex(0);
//                go.transform.localPosition = Vector3.zero;
//                go.transform.localRotation = Quaternion.identity;
//                _debugMap.Add(roadId, pathDebug);
//            }

//            foreach (Vector3 pos in posList)
//                pathDebug.AddPos(pos);

//            pathDebug.ReCalcRelativePoint();
//        }
//#endif
    }
}