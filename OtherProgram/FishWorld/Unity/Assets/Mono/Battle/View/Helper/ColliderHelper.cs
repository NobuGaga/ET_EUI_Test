// Battle Review

using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> 碰撞体辅助类, 避免反复获取创建相同的碰撞体 </summary>
    public static class ColliderHelper
    {
        /// <summary> 模型预设碰撞节点上限 </summary>
        private const ushort DefaultModelColliderCount = 8;

        private static Dictionary<int, ColliderMonoComponent> colliderComponentMap;

        private static List<ICollider> _colliderList;

        // Battle TODO delete
        private static List<CircCollider> _tempCircleColliderList;

        private static List<Transform> _bonesList;

        static ColliderHelper()
        {
            colliderComponentMap = new Dictionary<int, ColliderMonoComponent>(ConstHelper.FisheryUnitCount);
            _colliderList = new List<ICollider>(DefaultModelColliderCount);
            _bonesList = new List<Transform>(DefaultModelColliderCount);

            // Battle TODO delete
            _tempCircleColliderList = new List<CircCollider>(DefaultModelColliderCount);
        }

        public static void Clear()
        {
            colliderComponentMap.Clear();
            _colliderList.Clear();
            _bonesList.Clear();

            // Battle TODO delete
            _tempCircleColliderList.Clear();
        }

        public static ColliderMonoComponent GetColliderComponent(int colliderId, GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            if (colliderComponentMap.ContainsKey(instanceID))
                return colliderComponentMap[instanceID];

            var bonesTransformArray = GetModelBonesList(gameObject.transform, colliderId != 1);
            var colliderArray = CreateColliderList(colliderId);
            ColliderMonoComponent colliderComponent = new ColliderMonoComponent(colliderId, gameObject,
                                                                                colliderArray,
                                                                                bonesTransformArray);

            colliderComponentMap.Add(instanceID, colliderComponent);
            return colliderComponent;
        }

        private static ICollider[] CreateColliderList(int colliderId)
        {
            // Battle TODO 后面改成读取配置设置
            _colliderList.Clear();

            // Battle Warning 碰撞 ID 暂时使用鱼 ID, 子弹 Lua 层写死 1
            if (colliderId == 1)
                _colliderList.Add(new Line2D(ColliderConfig.CannonCamera, 2.6f, 3.35f));
            // Battle Warning 其他没有配置的默认给个半径为 1 的球
            else if (_tempCircleColliderList.Count == 0)
                _colliderList.Add(new Sphere(ColliderConfig.FishCamera, Vector3.zero, 1));
            else
            { 
                for (var index = 0; index < _tempCircleColliderList.Count; index++)
                {
                    var collider = _tempCircleColliderList[index];
                    _colliderList.Add(new Sphere(ColliderConfig.FishCamera, Vector3.zero, collider.radius));
                }
            }

            return _colliderList.CustomToArray();
        }

        public static Transform[] GetModelBonesList(Transform transform, bool isLogError = true)
        {
            _bonesList.Clear();

            // Battle TODO 设计一个模型脚本, 用来获取碰撞体挂点集合
            //Model model = transform.gameObject.GetComponent<Model>();

            _tempCircleColliderList.Clear();
            transform.gameObject.GetComponentsInChildren(_tempCircleColliderList);
            if (isLogError && _tempCircleColliderList.Count == 0)
            {
                Debug.LogErrorFormat("模型没有配置碰撞盒 {0}", transform.name);
            }

            for (var index = 0; index < _tempCircleColliderList.Count; index++)
            {
                var collider = _tempCircleColliderList[index];
                _bonesList.Add(collider.transform);
                collider.enabled = false;
            }

            // Battle Warning 子弹没有挂这个脚本
            if (_bonesList.Count == 0)
                _bonesList.Add(transform);

            Transform[] bonesList = _bonesList.CustomToArray();
            return bonesList;
        }

        // Battle Warning 原来的 List<T> 的 ToArray() 使用 Array.Copy() 可能有 GC, 修改实现观察看看
        private static T[] CustomToArray<T>(this List<T> list)
        {
            int listCount = list.Count;
            T[] array = new T[listCount];
            for (ushort index = 0; index < listCount; index++)
                array[index] = list[index];
            return array;
        }

        public static void RemoveGameObject(int instanceID)
        {
            if (!colliderComponentMap.ContainsKey(instanceID))
                return;

            ColliderMonoComponent colliderMonoComponent = colliderComponentMap[instanceID];
            colliderMonoComponent.Dispose();
            colliderComponentMap.Remove(instanceID);
        }

        private static Vector3 tempVector3;
        public static Vector3 GetScreenPoint(ushort cameraType, float worldPointX, float worldPointY,
                                                                float worldPointZ)
        {
            tempVector3.Set(worldPointX, worldPointY, worldPointZ);
            return GetScreenPoint(cameraType, ref tempVector3);
        }

        public static Vector3 GetScreenPoint(ushort cameraType, ref Vector3 worldPoint)
        {
            switch (cameraType)
            {
                case ColliderConfig.FishCamera:
                    return ReferenceHelper.FishCamera.WorldToScreenPoint(worldPoint);
                case ColliderConfig.CannonCamera:
                    return ReferenceHelper.CannoCamera.WorldToScreenPoint(worldPoint);
            }

            Log.Error($"ColliderHelper.GetScreenPoint cameraType error = { cameraType }");
            return Vector3.zero;
        }

        public static Vector3 GetWorldPoint(ushort cameraType, ref Vector3 screenPoint)
        {
            switch (cameraType)
            {
                case ColliderConfig.FishCamera:
                    return ReferenceHelper.FishCamera.ScreenToWorldPoint(screenPoint);
                case ColliderConfig.CannonCamera:
                    return ReferenceHelper.CannoCamera.ScreenToWorldPoint(screenPoint);
            }

            Log.Error($"ColliderHelper.GetWorldPoint cameraType error = { cameraType }");
            return Vector3.zero;
        }
    }
}