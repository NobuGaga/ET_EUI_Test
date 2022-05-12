#if !NOT_UNITY

using UnityEngine;

#endif

namespace ET
{
	public class FishMonoUnit : BattleMonoUnit
    {
        public FishMoveInfo FishMoveInfo;

        public FishScreenInfo FishScreenInfo;

        public TimeLineMonoInfo TimeLineMonoInfo;

        public void Init()
        {
            TimeLineMonoInfo = MonoPool.Instance.Fetch(typeof(TimeLineMonoInfo)) as TimeLineMonoInfo;
            TimeLineMonoInfo.Reset();
        }

        public override void FixedUpdate()
        {
            FishMoveInfoHelper.FixedUpdate(FishMoveInfo);

            if (!FishMoveInfo.IsMoveEnd)
                TransformInfo.Update(FishMoveInfo);

            if (TimeLineMonoInfo != null)
                TimeLineConfigInfoHelper.FixedUpdate(UnitId, TimeLineMonoInfo);
        }

#if !NOT_UNITY

        public override void Update()
        {
            if (Transform == null)
                return;

            Transform.localPosition = TransformInfo.LocalPosition;
            TransformInfo.WorldPosition = Transform.position;
            Transform.forward = TransformInfo.Forward;
            var animation = UnityComponentHelper.GetAnimation(Transform.gameObject);
            animation.Update((float)TimeHelper.ClinetDeltaFrameTime() / 1000);

            if (ColliderMonoComponent == null)
                return;

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
#if !NOT_UNITY

            if (Transform != null)
                UnityComponentHelper.GetAnimation(Transform.gameObject)?.Reset();
#endif
            base.Dispose();
            FishMoveInfo = null;
            FishScreenInfo = null;
            var timeLineMonoInfo = TimeLineMonoInfo;
            TimeLineMonoInfo = null;
            MonoPool.Instance.Recycle(timeLineMonoInfo);
        }
    }
}