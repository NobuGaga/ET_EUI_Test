#if !NOT_UNITY

using UnityEngine;

#endif

namespace ET
{
    /// <summary> Mono 层战斗 Unit 类 </summary>
	public abstract class BattleMonoUnit
    {
        public TransformInfo TransformInfo;

        public abstract void FixedUpdate();

        public virtual void Dispose()
        {
            TransformInfo = null;
#if NOT_UNITY
        }
#else
            Transform = null;
            ColliderMonoComponent = null;
        }

        public Transform Transform;

        public ColliderMonoComponent ColliderMonoComponent;

        public abstract void Update();
#endif
    }
}