// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public class BulletUnitComponentAwakeSystem : AwakeSystem<BulletUnitComponent, CannonShootInfo>
    {
        public override void Awake(BulletUnitComponent self, CannonShootInfo cannonShootInfo)
        {
            BulletMoveInfo bulletMoveInfo = BulletMoveHelper.PopInfo();
            bulletMoveInfo.Reset();

            BulletMoveHelper.InitInfo(bulletMoveInfo, cannonShootInfo);
            CannonShootHelper.PushPool(cannonShootInfo);
            self.Info = bulletMoveInfo;

            Unit bulletUnit = self.Parent as Unit;
            self.TrackFishUnitId = bulletUnit.AttributeComponent[NumericType.TrackFishId];
            ref long trackFishUnitId = ref self.TrackFishUnitId;

            if (trackFishUnitId != BulletConfig.DefaultTrackFishUnitId)
            {
                var unitComponent = self.DomainScene().GetComponent<UnitComponent>();
                Unit fishUnit = unitComponent.Get(trackFishUnitId);
                self.SetTrackDirection(fishUnit.FishUnitComponent.AimPoint.Vector);
            }

            bulletUnit.TransformComponent.NodeName = string.Format(BulletConfig.NameFormat, bulletUnit.UnitId);
            bulletUnit.TransformComponent.Info.Update(bulletMoveInfo);
        }
    }

    [ObjectSystem]
    public class BulletUnitComponentDestroySystem : DestroySystem<BulletUnitComponent>
    {
        public override void Destroy(BulletUnitComponent self)
        {
            BulletMoveHelper.PushPool(self.Info);
            self.Info = null;
        }
    }

    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public static class BulletUnitComponentSystem
    {
        internal static void FixedUpdate(this BulletUnitComponent self, Unit unit)
        {
            if (self.TrackFishUnitId != BulletConfig.DefaultTrackFishUnitId)
            {
                Unit fishUnit = SkillHelper.GetTrackFishUnit(unit.DomainScene(), self.TrackFishUnitId);
                if (fishUnit == null)
                {
                    unit.AttributeComponent.Set(NumericType.TrackFishId, BulletConfig.DefaultTrackFishUnitId);
                    self.TrackFishUnitId = BulletConfig.DefaultTrackFishUnitId;
                }
                else
                    self.SetTrackDirection(fishUnit.FishUnitComponent.AimPoint.Vector);
            }
            
            BulletMoveInfo info = self.Info;
            BulletMoveHelper.FixedUpdate(info);
            unit.TransformComponent.Info.Update(info);
        }

        internal static void SetTrackDirection(this BulletUnitComponent self, Vector3 trackScreenPos)
        {
            // 追踪鱼重新计算方向, 在逻辑层设置好追踪屏幕位置
            BulletMoveInfo info = self.Info;
            Vector2 moveDirection = info.MoveDirection;
            float moveDirectionX = trackScreenPos.x - info.ScreenPos.x;
            if (Mathf.Abs(moveDirectionX) > BulletConfig.TrackDirectionFix)
                moveDirection.x = moveDirectionX;

            float moveDirectionY = trackScreenPos.y - info.ScreenPos.y;
            if (Mathf.Abs(moveDirectionY) > BulletConfig.TrackDirectionFix)
                moveDirection.y = moveDirectionY;

            moveDirection.Normalize();
            info.MoveDirection = moveDirection;
        }
    }
}