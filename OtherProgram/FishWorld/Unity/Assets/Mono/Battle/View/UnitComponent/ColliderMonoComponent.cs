using UnityEngine;

namespace ET
{
	// 碰撞体视图组件, 用来视图层碰撞相关数据跟骨骼节点
	public class ColliderMonoComponent
	{
        /// <summary> 碰撞配置 ID </summary>
        private int colliderId;

        /// <summary> 模型根节点 </summary>
        private Transform rootTransform;

        private float scale;

        /// <summary> 碰撞体数组 </summary>
        private ICollider[] colliderArray;

		/// <summary> 骨骼节点数组 </summary>
		private Transform[] bonesTransformArray;

        /// <summary> 子弹移动方向, 单位向量 </summary>
        private Vector2 moveDirection;

        public ColliderMonoComponent(int colliderId, GameObject gameObject, ICollider[] colliderArray,
                                     Transform[] bonesTransformArray)
        {
            this.colliderId = colliderId;
            rootTransform = gameObject.transform;
            scale = rootTransform.localScale.x;
            this.colliderArray = colliderArray;
            this.bonesTransformArray = bonesTransformArray;

            if (colliderArray == null || bonesTransformArray == null)
            {
                string msg = $"new ColliderMonoComponent error colliderId = { colliderId }, collider or bone is null";
                throw new System.Exception(msg);
            }
        }

        public void Dispose()
        {
            rootTransform = null;
            colliderArray = null;
            bonesTransformArray = null;
        }

        public void SetMoveDirection(Vector2 moveDirection) =>
                    this.moveDirection.Set(moveDirection.x, moveDirection.y);

        public void UpdateColliderCenter()
        {
            for (int index = 0; index < colliderArray.Length; index++)
            {
                ICollider collider = colliderArray[index];
                Vector3 boneWorldPosition = bonesTransformArray[index].position;

                if (!(collider is Line2D))
                {
                    // Battle Warning Sphere 会修改 boneWorldPosition 的值
                    // 这里后面执行的逻辑不能使用 boneWorldPosition
                    collider.Update(ref boneWorldPosition, scale);
                    continue;
                }

                Line2D line = collider as Line2D;
                line.Update(ref boneWorldPosition, scale, ref moveDirection);
#if UNITY_EDITOR

                line.AddLineDrawData();
#endif
            }
        }

        public bool IsCollide(ColliderMonoComponent other)
        {
            ICollider[] selfColliderList = colliderArray;
            ICollider[] otherColliderList = other.colliderArray;

            if (selfColliderList == null || otherColliderList == null)
                return false;

            for (var selfIndex = 0; selfIndex < selfColliderList.Length; selfIndex++)
            {
                for (var otherIndex = 0; otherIndex < otherColliderList.Length; otherIndex++)
                {
                    if (selfColliderList[selfIndex].IsCollide(otherColliderList[otherIndex]))
                        return true;
                }
            }

            return false;
        }

        public Vector3 GetBulletCollidePoint()
        {
            if (bonesTransformArray == null || bonesTransformArray.Length == 0 ||
                colliderArray == null || colliderArray.Length == 0)
            {
                Vector3 worldPosition = rootTransform.position;
                return ColliderHelper.GetScreenPoint(ColliderConfig.CannonCamera, ref worldPosition);
            }

            Line2D line = (Line2D)colliderArray[0];
            ref Vector2 endPos = ref line.EndPos;
            ref float centerZ = ref line.CenterZ;
            return new Vector3(endPos.x, endPos.y, centerZ);
        }

        public Vector3 GetFishAimPoint()
        {
            Vector3 worldPosition;
            if (bonesTransformArray == null || bonesTransformArray.Length == 0)
            {
                worldPosition = rootTransform.position;
                return ColliderHelper.GetScreenPoint(ColliderConfig.FishCamera, ref worldPosition);
            }

            int length = (bonesTransformArray.Length + 1) / 2 - 1;
            Transform pointNode = bonesTransformArray[length];
            worldPosition = pointNode.position;
            return ColliderHelper.GetScreenPoint(ColliderConfig.FishCamera, ref worldPosition);
        }
    }
}