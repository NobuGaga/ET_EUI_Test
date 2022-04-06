#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    internal static class BattleDebug
    {
        private struct LineDrawData
        {
            public Vector3 from;
            public Vector3 to;

            public LineDrawData(Vector3 from, Vector3 to)
            {
                this.from = from;
                this.to = to;
            }
        }

        private static LineDrawData _lineDataCache = default;
        private static List<LineDrawData> _listLineData = new List<LineDrawData>(2 ^ 9);

        public static void AddLineDrawData(Vector3 from, Vector3 to)
        {
            _lineDataCache.from = from;
            _lineDataCache.to = to;
            _listLineData.Add(_lineDataCache);
        }

        public static void Clear() => _listLineData.Clear();

        public static void OnDrawGizmosCallBack()
        {
            if (Camera.current.name != ColliderConfig.CannonCameraName)
                return;

            foreach (LineDrawData data in _listLineData)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(data.from, 3);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(data.to, 3);
            }
        }
    }
}

#endif