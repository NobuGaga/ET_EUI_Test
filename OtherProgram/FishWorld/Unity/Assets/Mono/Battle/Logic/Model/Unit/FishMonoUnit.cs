#if !NOT_UNITY

using UnityEngine;

#endif

namespace ET
{
	public class FishMonoUnit : BattleMonoUnit
    {
        public FishMoveInfo FishMoveInfo;

        public FishScreenInfo FishScreenInfo;

        public override void FixedUpdate()
        {
            FishMoveInfoHelper.FixedUpdate(FishMoveInfo);

            if (!FishMoveInfo.IsMoveEnd)
                TransformInfo.Update(FishMoveInfo);
        }

#if !NOT_UNITY

        public override void Update()
        {
            if (Transform != null)
            {
                Transform.localPosition = TransformInfo.LocalPosition;
                TransformInfo.WorldPosition = Transform.position;
                Transform.forward = TransformInfo.Forward;
            }

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
            base.Dispose();
            FishMoveInfo = null;
            FishScreenInfo = null;
        }
    }
}