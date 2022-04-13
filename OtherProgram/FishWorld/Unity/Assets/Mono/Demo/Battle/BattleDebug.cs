#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    internal static class BattleDebug
    {
        private static Dictionary<short, PathDebug> _debugMap = new Dictionary<short, PathDebug>(FishPathHelper.DefaultPathCount);

        // 开启鱼线调试, 使用 Gizmos 画线
        public static void OpenPathDebug(short roadId, Vector3[] posList)
        {
            PathDebug pathDebug;
            if (_debugMap.ContainsKey(roadId))
            {
                pathDebug = _debugMap[roadId];
                pathDebug.ClearList();
            }
            else
            {
                GameObject go = new GameObject();
                go.name = string.Format("PathDebug_{0}", roadId);
                pathDebug = go.AddComponent<PathDebug>();
                go.transform.SetParent(ReferenceHelper.FishRootNode.transform);
                go.transform.SetSiblingIndex(0);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                _debugMap.Add(roadId, pathDebug);
            }

            for (var index = 0; index < posList.Length; index++)
                pathDebug.AddPos(posList[index]);

            pathDebug.ReCalcRelativePoint();
        }

        #region Draw Bullet Line

        private struct LineDrawData
        {
            public Vector3 StartPoint;
            public Vector3 EndPoint;
        }

        private static LineDrawData lineDataCache;
        private static List<LineDrawData> lineDataList = new List<LineDrawData>(2 ^ 9);

        public static void AddBulletDrawData(Vector3 startPoint, Vector3 endPoint)
        {
            lineDataCache.StartPoint = startPoint;
            lineDataCache.EndPoint = endPoint;
            lineDataList.Add(lineDataCache);
        }

        public static void Clear() => lineDataList.Clear();

        private static void DrawLineBullet()
        {
            if (Camera.current.name != ColliderConfig.CannonCameraName)
                return;

            for (var index = 0; index < lineDataList.Count; index++)
            {
                LineDrawData data = lineDataList[index];

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(data.StartPoint, 3);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(data.EndPoint, 3);
            }

            Gizmos.color = Color.white;
        }

        #endregion

        public static void OnDrawGizmosCallBack() => DrawLineBullet();
    }
}

#endif