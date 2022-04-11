// Battle Review

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

        /// <summary> 碰撞体数组 </summary>
        public ICollider[] ColliderList;

		/// <summary> 骨骼节点数组 </summary>
		public Transform[] BonesTransList;

        /// <summary> 子弹移动方向, 单位向量 </summary>
        private Vector2 moveDirection;

        public ColliderMonoComponent(int colliderId, GameObject gameObject)
        {
            this.colliderId = colliderId;
            rootTransform = gameObject.transform;
        }

        public void Dispose()
        {
            colliderId = 0;
            rootTransform = null;
            ColliderList = null;
            BonesTransList = null;
        }

        public void SetMoveDirection(Vector2 moveDirection) =>
                    this.moveDirection.Set(moveDirection.x, moveDirection.y);

        public void UpdateColliderCenter()
        {
            if (ColliderList == null || BonesTransList == null)
                return;

            float scale = rootTransform.localScale.x;
            for (ushort index = 0; index < ColliderList.Length; index++)
            {
                ref ICollider collider = ref ColliderList[index];
                Transform bone = BonesTransList[index];
                Vector3 boneWorldPosition = bone.position;
                collider.Update(ref boneWorldPosition, scale);
                if (collider is Line2D && colliderId == 1)
                {
                    Line2D line = (Line2D)collider;
                    line.Update(ref boneWorldPosition, scale, ref moveDirection);
#if UNITY_EDITOR
                    line.AddLineDrawData();
#endif
                    collider = line;
                }
            }
        }

        public bool IsCollide(ColliderMonoComponent other)
        {
            ICollider[] selfColliderList = ColliderList;
            ICollider[] otherColliderList = other.ColliderList;

            if (selfColliderList == null || otherColliderList == null)
                return false;

            foreach (ICollider selfCollider in selfColliderList)
            {
                foreach (ICollider otherCollider in otherColliderList)
                {
                    if (selfCollider.IsCollide(otherCollider))
                        return true;
                }
            }

            return false;
        }

        public Vector3 GetBulletCollidePoint()
        {
            if (BonesTransList == null || BonesTransList.Length == 0 ||
                ColliderList == null || ColliderList.Length == 0)
            {
                Vector3 worldPosition = rootTransform.position;
                return ColliderHelper.GetScreenPoint(ColliderConfig.CannonCamera, ref worldPosition);
            }

            Line2D line = (Line2D)ColliderList[0];
            ref Vector2 endPos = ref line.EndPos;
            ref float centerZ = ref line.CenterZ;
            return new Vector3(endPos.x, endPos.y, centerZ);
        }

        public Vector3 GetFishAimPoint()
        {
            Vector3 worldPosition;
            if (BonesTransList == null || BonesTransList.Length == 0)
            {
                worldPosition = rootTransform.position;
                return ColliderHelper.GetScreenPoint(ColliderConfig.FishCamera, ref worldPosition);
            }

            int length = (BonesTransList.Length + 1) / 2 - 1;
            Transform pointNode = BonesTransList[length];
            worldPosition = pointNode.position;
            return ColliderHelper.GetScreenPoint(ColliderConfig.FishCamera, ref worldPosition);
        }
    }
}