using UnityEngine;

namespace ET
{
    public struct Line2DOffset
    {
        public float Start;
        public float End;

        public Line2DOffset(float startOffset, float endOffset)
        {
            Start = startOffset; End = endOffset;
        }
    }

    public struct Line2D : ICollider
    {
        /// <summary> 摄像机类型(固定值) </summary>
        private ushort cameraType;

        /// <summary> 骨骼点世界坐标(根据上一帧骨骼点更新) </summary>
        private Vector2 center;
        private float centerZ;

        /// <summary> 
        /// 偏移值, 线碰撞只做开始位置跟结束位置 Y 轴的偏移
        /// 偏移值只在初始化时赋值一次
        /// </summary>
        private Line2DOffset offset;

        /// <summary> 线段开始位置(屏幕坐标) </summary>
        private Vector2 startPos;
        /// <summary> 线段结束位置(屏幕坐标) </summary>
        private Vector2 endPos;
        /// <summary> 开始坐标跟结束坐标的距离 </summary>
        private Vector2 distance;

        public Line2D(ushort cameraType, float startOffset, float endOffset)
        {
            this.cameraType = cameraType;

            center = ColliderConfig.DefaultCenter;
            centerZ = ColliderConfig.DefaultCenter.z;

            offset = new Line2DOffset(startOffset, endOffset);
            
            startPos = new Vector2(0, -startOffset);
            endPos = new Vector2(0, endOffset);

            distance = new Vector2(0, startOffset + endOffset);
        }

        public bool IsCollide(ref Sphere sphere)
        {
            Vector3 center = sphere.Center;
            return IsCollide(ref center, sphere.Radius);
        }

        public bool IsCollide(ref Point3D point)
        {
            Vector3 center = point.Center;
            return IsCollide(ref center, 0);
        }

        private bool IsCollide(ref Vector3 point, float maxDis)
        {
            // 先计算跟线段两个端点距离是否小于半径
            // 都大于则计算在线段直线方向上的投影点是否在线段上
            // 在线段上则计算跟投影点的距离对比球体半径
            float minDisSquare = maxDis * maxDis;

            float pointX = point.x;
            float pointY = point.y;

            float diffX = startPos.x - pointX;
            float diffY = startPos.y - pointY;

            if (diffX * diffX + diffY * diffY < minDisSquare)
                return true;

            diffX = endPos.x - pointX;
            diffY = endPos.y - pointY;

            if (diffX * diffX + diffY * diffY < minDisSquare)
                return true;

            float sqrtMagnitudeSe = distance.sqrMagnitude;

            // 跟 startPos 是否相交, 上面算过
            if (sqrtMagnitudeSe < Vector2.kEpsilon)
                return false;

            float dotSpSe = ((pointX - startPos.x) * distance.x + (pointY - startPos.y) * distance.y) / sqrtMagnitudeSe;

            float linePointX = distance.x * dotSpSe + startPos.x;
            float linePointY = distance.y * dotSpSe + startPos.y;

            float minX = Mathf.Min(startPos.x, endPos.x);
            float minY = Mathf.Min(startPos.y, endPos.y);
            float maxX = Mathf.Max(startPos.x, endPos.x);
            float maxY = Mathf.Max(startPos.y, endPos.y);

            if (linePointX < minX || linePointX > maxX || linePointY < minY || linePointY > maxY)
                return false;

            diffX = pointX - linePointX;
            diffY = pointY - linePointY;

            return diffX * diffX + diffY * diffY < minDisSquare;
        }

        #region ICollider

        public void Update(ref Vector3 boneWorldPoint, float scale) =>
                    Update(ref boneWorldPoint, scale, ref ColliderConfig.DefaultLineDirection);

        #endregion

        public void Update(ref Vector3 boneWorldPoint, float scale, ref Vector2 direction)
        {
            center.Set(boneWorldPoint.x, boneWorldPoint.y);
            centerZ = boneWorldPoint.z;

            // 计算屏幕坐标, 赋值完中心点, 缩放值, 方向后进行
            float directionX = direction.x * scale;
            float directionY = direction.y * scale;

            Vector3 screenPos = ColliderHelper.GetScreenPoint(cameraType, center.x - directionX * offset.Start,
                                                                          center.y - directionY * offset.Start,
                                                                          centerZ);
            startPos.Set(screenPos.x, screenPos.y);

            screenPos = ColliderHelper.GetScreenPoint(cameraType, center.x + directionX * offset.End,
                                                                  center.y + directionY * offset.End,
                                                                  centerZ);
            endPos.Set(screenPos.x, screenPos.y);

            distance.Set(endPos.x - startPos.x, endPos.y - startPos.y);
        }
    }

    public static class Line2DExtension
    {
        public static bool IsCollide(this ref Sphere sphere, ref Line2D line) =>
                           line.IsCollide(ref sphere);

        public static bool IsCollide(this ref Point3D point, ref Line2D line) =>
                           line.IsCollide(ref point);
    }
}