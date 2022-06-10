using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public class BulletUnitComponentAwakeSystem : AwakeSystem<BulletUnitComponent, CannonShootInfo>
    {
        public override void Awake(BulletUnitComponent self, CannonShootInfo cannonShootInfo)
        {
            Unit bulletUnit = self.Parent as Unit;
            BulletMoveInfo bulletMoveInfo = BulletMoveHelper.PopInfo(bulletUnit.UnitId);
            bulletMoveInfo.Reset();

            BulletMoveHelper.InitInfo(bulletMoveInfo, cannonShootInfo);
            CannonShootHelper.PushPool(cannonShootInfo);
            self.Info = bulletMoveInfo;

            self.Info.TrackFishUnitId = bulletUnit.AttributeComponent[NumericType.TrackFishId];
            ref long trackFishUnitId = ref self.Info.TrackFishUnitId;

            if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId)
            {
                var unitComponent = BattleLogicComponent.Instance.UnitComponent;
                Unit fishUnit = unitComponent.Get(trackFishUnitId);
                self.SetTrackDirection(fishUnit.FishUnitComponent.ScreenInfo.AimPoint);
            }

            if (ConstHelper.IsEditor)
                bulletUnit.TransformComponent.NodeName = string.Format(BulletConfig.NameFormat, bulletUnit.UnitId);

            bulletUnit.TransformComponent.Info.Update(bulletMoveInfo);
        }
    }

    [ObjectSystem]
    public class BulletUnitComponentDestroySystem : DestroySystem<BulletUnitComponent>
    {
        public override void Destroy(BulletUnitComponent self) => self.Info = null;
    }

    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public static class BulletUnitComponentSystem
    {
        internal static void SetTrackDirection(this BulletUnitComponent self, Vector3 trackScreenPos)
        {
            // 追踪鱼重新计算方向, 在逻辑层设置好追踪屏幕位置
            BulletMoveInfo info = self.Info;
            Vector2 moveDirection = info.MoveDirection;
            float moveDirectionX = trackScreenPos.x - info.ScreenPos.x;
            if (Mathf.Abs(moveDirectionX) > ConstHelper.TrackDirectionFix)
                moveDirection.x = moveDirectionX;

            float moveDirectionY = trackScreenPos.y - info.ScreenPos.y;
            if (Mathf.Abs(moveDirectionY) > ConstHelper.TrackDirectionFix)
                moveDirection.y = moveDirectionY;

            moveDirection.Normalize();
            info.MoveDirection = moveDirection;
        }
    }
}