#if !NOT_UNITY

using UnityEngine;

#endif

namespace ET
{
	public class FishMonoUnit : BattleMonoUnit
    {
        public LifeCycleInfo LifeCycleInfo;

        public FishMoveInfo FishMoveInfo;

        public FishScreenInfo FishScreenInfo;

        public TimeLineMonoInfo TimeLineMonoInfo;

        public TransformRotateInfo TransformRotateInfo;

        public void Init()
        {
            LifeCycleInfo = MonoPool.Instance.Fetch(typeof(LifeCycleInfo)) as LifeCycleInfo;
            LifeCycleInfo.Reset();
            TimeLineMonoInfo = MonoPool.Instance.Fetch(typeof(TimeLineMonoInfo)) as TimeLineMonoInfo;
            TimeLineMonoInfo.Reset();
        }

        public override void FixedUpdate()
        {
            LifeCycleInfoHelper.FixedUpdate(LifeCycleInfo, FishMoveInfo);

            if (TimeLineMonoInfo != null)
                TimeLineConfigInfoHelper.FixedUpdate(UnitId, LifeCycleInfo, TimeLineMonoInfo);

            FishMoveInfoHelper.FixedUpdate(FishMoveInfo);
            if (!FishMoveInfo.IsMoveEnd)
                TransformInfo.Update(FishMoveInfo);

            if (TransformRotateInfo.IsRotating)
                TransformRotateInfoHelper.FixedUpdate(TransformInfo, TransformRotateInfo);
        }

#if !NOT_UNITY

        public override void Update()
        {
            if (Transform == null) return;

            float deltaTime = Time.deltaTime;

            Transform.localPosition = TransformInfo.LocalPosition;
            TransformInfo.WorldPosition = Transform.position;

            if (TransformRotateInfo.IsFowardMainCamera)
                Transform.LookAt(ReferenceHelper.FishCamera.transform.position);
            else
                Transform.forward = Vector3.Slerp(Transform.forward, TransformInfo.Forward, deltaTime);

            if (TransformRotateInfo.IsRotating)
            {
                Transform.localRotation = TransformInfo.LocalRotation;
                TransformInfo.LocalRotation = Transform.localRotation;
            }

            var animation = UnityComponentHelper.GetAnimation(Transform.gameObject);
            animation.Update(deltaTime);

            if (ColliderMonoComponent == null) return;

            ColliderMonoComponent.UpdateColliderCenter();
            FishScreenInfo.AimPoint = ColliderMonoComponent.GetFishAimPoint();
            ref var aimPointPos = ref FishScreenInfo.AimPoint;
            FishScreenInfo.IsInScreen = aimPointPos.x > 0 && aimPointPos.y > 0 &&
                                        aimPointPos.x < Screen.width &&
                                        aimPointPos.y < Screen.height;
        }
#endif

        public override void Dispose()
        {
            base.Dispose();
            MonoPool.Instance.Recycle(LifeCycleInfo);
            LifeCycleInfo = null;
            FishMoveInfo = null;
            FishScreenInfo = null;
            MonoPool.Instance.Recycle(TimeLineMonoInfo);
            TimeLineMonoInfo = null;
#if !NOT_UNITY

            if (Transform != null)
                UnityComponentHelper.GetAnimation(Transform.gameObject)?.Reset();
#endif
        }
    }
}