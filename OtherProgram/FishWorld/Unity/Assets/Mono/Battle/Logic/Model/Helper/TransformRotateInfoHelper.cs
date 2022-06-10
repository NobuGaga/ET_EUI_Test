// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    /// <summary>
    /// 鱼移动数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用
    /// 用于处理复杂的鱼线计算
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class TransformRotateInfoHelper
    {
        public static TransformRotateInfo PopInfo(long unitId)
        {
            var unit = UnitMonoComponent.Instance.Get<FishMonoUnit>(unitId);
            var info = unit.TransformRotateInfo;
            if (info != null)
                return info;

            info = MonoPool.Instance.Fetch(typeof(TransformRotateInfo)) as TransformRotateInfo;
            unit.TransformRotateInfo = info;
            return info;
        }

        public static void FixedUpdate(TransformInfo transformInfo, TransformRotateInfo rotateInfo)
        {
            int deltaTime = (int)TimeHelper.ClinetDeltaFrameTime();
            rotateInfo.RotationTime += deltaTime;
            float rate = (float)rotateInfo.RotationTime / rotateInfo.RotationDuration;
            if (rate > 1)
            {
                rotateInfo.IsRotating = false;
                return;
            }

            var eulerAngles = ToEulerAngle(transformInfo.LocalRotation);
            eulerAngles.z += (float)deltaTime / rotateInfo.RotationDuration * rotateInfo.LocalRotationZ;
            transformInfo.LocalRotation = ToQuaternion(eulerAngles);
        }

        public static Vector3 ToEulerAngle(Quaternion quaternion)
        {
            ref float x = ref quaternion.x;
            ref float y = ref quaternion.y;
            ref float z = ref quaternion.z;
            ref float w = ref quaternion.w;

            var vector = new Vector3();
            vector.x = Mathf.Asin(2 * (w * x - y * z));
            vector.y = Mathf.Atan2(2 * (w * y + x * z), 1 - 2 * (x * x + y * y));
            vector.z = Mathf.Atan2(2 * (w * z + x * y), 1 - 2 * (x * x + z * z));

            return vector;
        }

        public static Quaternion ToQuaternion(Vector3 vector)
        {
            float halfX = vector.x / 2;
            float halfY = vector.y / 2;
            float halfZ = vector.z / 2;

            float sinHalfX = Mathf.Sin(halfX);
            float cosHalfX = Mathf.Cos(halfX);

            float sinHalfY = Mathf.Sin(halfY);
            float cosHalfY = Mathf.Cos(halfY);

            float sinHalfZ = Mathf.Sin(halfZ);
            float cosHalfZ = Mathf.Cos(halfZ);

            var quaternion = new Quaternion();
            quaternion.x = sinHalfY * sinHalfZ * cosHalfX + cosHalfY * cosHalfZ * sinHalfX;
            quaternion.y = sinHalfY * cosHalfZ * cosHalfX - cosHalfY * sinHalfZ * sinHalfX;
            quaternion.z = cosHalfY * sinHalfZ * cosHalfX - sinHalfY * cosHalfZ * sinHalfX;
            quaternion.w = cosHalfX * cosHalfY * cosHalfZ - sinHalfX * sinHalfY * sinHalfZ;

            return quaternion;
        }
    }
}