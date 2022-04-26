using UnityEngine;

namespace ET
{
	public class BulletMonoUnit : BattleMonoUnit
    {
        public BulletMoveInfo BulletMoveInfo;

        public override void FixedUpdate()
        {
            ref var trackFishUnitId = ref BulletMoveInfo.TrackFishUnitId;
            if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId)
            {
                var fishUnit = UnitMonoComponent.Instance.Get<FishMonoUnit>(trackFishUnitId);
                if (fishUnit == null)
                    trackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
                else
                {
                    ref Vector3 trackScreenPos = ref fishUnit.FishScreenInfo.AimPoint;
                    ref Vector2 moveDirection = ref BulletMoveInfo.MoveDirection;
                    ref Vector3 screenPosition = ref BulletMoveInfo.ScreenPos;
                    float moveDirectionX = trackScreenPos.x - screenPosition.x;
                    if (Mathf.Abs(moveDirectionX) > ConstHelper.TrackDirectionFix)
                        moveDirection.x = moveDirectionX;

                    float moveDirectionY = trackScreenPos.y - screenPosition.y;
                    if (Mathf.Abs(moveDirectionY) > ConstHelper.TrackDirectionFix)
                        moveDirection.y = moveDirectionY;

                    moveDirection.Normalize();
                }
            }

            BulletMoveHelper.FixedUpdate(BulletMoveInfo);
            TransformInfo.Update(BulletMoveInfo);
        }

#if !NOT_UNITY

        public override void Update()
        {
            Camera camera = ReferenceHelper.CannoCamera;
            if (Transform != null)
            {
                Transform.localPosition = TransformInfo.LocalPosition;
                TransformInfo.WorldPosition = Transform.position;
                Transform.localRotation = TransformInfo.LocalRotation;
                TransformInfo.Rotation = Transform.rotation;
                BulletMoveInfo.ScreenPos = camera.WorldToScreenPoint(Transform.position);
            }
            else
                BulletMoveInfo.ScreenPos = camera.WorldToScreenPoint(TransformInfo.WorldPosition);

            ColliderMonoComponent?.UpdateColliderCenter();
        }
#endif

        public override void Dispose()
        {
            base.Dispose();
            BulletMoveInfo = null;
        }
    }
}