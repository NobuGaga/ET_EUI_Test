using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ExecuteInEditMode]
    public class PathDebug : MonoBehaviour
    {
        public Color color1 = new Color(1, 0, 1, 0.5f);
        public Color color2 = new Color(1, 235 / 255f, 4 / 255f, 0.5f);
        public Vector3 size = new Vector3(.7f, .7f, .7f);
        public float radius = .4f;
        public bool drawCurved = true;

        public bool calcRelativePos = false;
        // 相对坐标
        public List<Vector3> posList = new List<Vector3>(16);

        public List<Vector3> relativePosList = new List<Vector3>(16);

        List<Vector3> _pathPoint = new List<Vector3>(16);

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (calcRelativePos)
            {
                ReCalcRelativePoint();
                calcRelativePos = false;
            }

            if (relativePosList.Count <= 0)
                return;

            Vector3 start = relativePosList[0];
            Vector3 end = relativePosList[relativePosList.Count - 1];
            Gizmos.color = color1;
            Gizmos.DrawWireCube(start, size * GetHandleSize(start) * 1.5f);
            Gizmos.DrawWireCube(end, size * GetHandleSize(end) * 1.5f);

            Gizmos.color = Color.white;   //

            //if (selectName == gameObject.name)
            //{
            //    Gizmos.color = color2;
            //}

            for (int i = 1; i < relativePosList.Count - 1; i++)
                Gizmos.DrawWireSphere(relativePosList[i], radius * GetHandleSize(relativePosList[i]));

            //draw linear or curved lines with the same color
            if (drawCurved && relativePosList.Count >= 2)
                DrawCurved(relativePosList);
            else
                DrawStraight(relativePosList);
        }

#endif

        float GetHandleSize(Vector3 pos)
        {
            float handleSize = 1f;
#if UNITY_EDITOR
            handleSize = UnityEditor.HandleUtility.GetHandleSize(pos) * 0.4f;
            handleSize = Mathf.Clamp(handleSize, 0, 1.2f);
#endif
            return handleSize;
        }

        void DrawCurved(List<Vector3> pathPoints)
        {
            if (_pathPoint.Count <= 0)
                _pathPoint = GetCurved(pathPoints);


            //pathPoints = GetCurved(pathPoints);
            Vector3 prevPt = _pathPoint[0];
            Vector3 currPt;

            for (int i = 1; i < _pathPoint.Count; ++i)
            {
                currPt = _pathPoint[i];
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

        static List<Vector3> GetCurved(List<Vector3> waypoints)
        {
            //helper array for curved paths, includes control points for waypoint array
            Vector3[] gizmoPoints = new Vector3[waypoints.Count + 2];
            waypoints.CopyTo(gizmoPoints, 1);
            gizmoPoints[0] = waypoints[1];
            gizmoPoints[gizmoPoints.Length - 1] = gizmoPoints[gizmoPoints.Length - 2];

            //Vector3[] drawPs;
            Vector3 currPt;

            //store draw points
            int subdivisions = gizmoPoints.Length * 10;

            List<Vector3> drawPs = new List<Vector3>(subdivisions + 1);

            //drawPs = new Vector3[subdivisions + 1];
            for (int i = 0; i <= subdivisions; ++i)
            {
                float pm = i / (float)subdivisions;
                currPt = GetPoint(gizmoPoints, pm);
                //if (drawPs[i] != null)
                //drawPs[i] = currPt;
                //else
                drawPs.Add(currPt);
            }

            return drawPs;
        }

        static Vector3 GetPoint(Vector3[] gizmoPoints, float t)
        {
            int numSections = gizmoPoints.Length - 3;
            int tSec = (int)Mathf.Floor(t * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec)
            {
                currPt = tSec;
            }
            float u = t * numSections - currPt;

            Vector3 a = gizmoPoints[currPt];
            Vector3 b = gizmoPoints[currPt + 1];
            Vector3 c = gizmoPoints[currPt + 2];
            Vector3 d = gizmoPoints[currPt + 3];

            return .5f * (
                           (-a + 3f * b - 3f * c + d) * (u * u * u)
                           + (2f * a - 5f * b + 4f * c - d) * (u * u)
                           + (-a + c) * u
                           + 2f * b
                       );
        }

        static void DrawStraight(List<Vector3> waypoints)
        {
            for (int i = 0; i < waypoints.Count - 1; i++)
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }


        public void ReCalcRelativePoint()
        {
            relativePosList.Clear();
            foreach (var pos in posList)
            {
                relativePosList.Add(pos);
            }

            for (int i = 0; i < posList.Count; i++)
            {
                relativePosList[i] = transform.TransformPoint(posList[i]);
            }
            _pathPoint.Clear();
        }
        public void ClearList()
        {
            _pathPoint.Clear();
            relativePosList.Clear();
            posList.Clear();
        }

        public void AddPos(Vector3 pos)
        {
            posList.Add(pos);
        }
    }

}
