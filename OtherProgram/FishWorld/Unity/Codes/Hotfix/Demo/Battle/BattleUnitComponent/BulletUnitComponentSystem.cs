using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class BulletUnitComponentAwakeSystem : AwakeSystem<BulletUnitComponent, CannonShootInfo>
    {
        public override void Awake(BulletUnitComponent self, CannonShootInfo cannonShootInfo)
        {
            Unit unit = self.Parent as Unit;

            BulletMoveInfo bulletMoveInfo = BulletMoveHelper.PopInfo();
            bulletMoveInfo.Reset();
            BulletMoveHelper.InitInfo(bulletMoveInfo, cannonShootInfo);

            CannonShootHelper.PushPool(cannonShootInfo);
            self.Info = bulletMoveInfo;
            self.InitTrackPosition();

            self.InitTransform();
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

    public static class BulletUnitComponentSystem
    {
        public static void FixedUpdate(this BulletUnitComponent self)
        {
            BulletMoveInfo info = self.Info;
            self.UpdateTrackPosition();
            BulletMoveHelper.FixedUpdate(info);
            self.UpdateTransform();
        }

        public static void InitTransform(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            transformComponent.NodeName = self.GetNodeName();
            self.UpdateTransform();
        }

        private static void UpdateTransform(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
            BulletMoveInfo info = self.Info;
            transformComponent.SetLocalPos(info.CurrentLocalPos);
            transformComponent.SetLocalRotation(info.CurrentRotation);
        }

        private static string GetNodeName(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            return string.Format(BulletConfig.NameFormat, unit.Id);
        }

        public static bool IsTrackBullet(this BulletUnitComponent self) =>
                           self.GetTrackFishUnitId() == BulletConfig.DefaultTrackFishUnitId;

        public static long GetTrackFishUnitId(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            var attributeComponent = unit.GetComponent<NumericComponent>();
            return attributeComponent[NumericType.TrackFishId];
        }

        public static void InitTrackPosition(this BulletUnitComponent self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();
            if (trackFishUnitId == BulletConfig.DefaultTrackFishUnitId)
                return;

            Scene currentScene = self.DomainScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);
            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            self.SetTrackDirection(fishUnitComponent.AimPoint.Vector);
        }

        /// <summary> 更新追踪鱼的屏幕坐标 </summary>
        private static void UpdateTrackPosition(this BulletUnitComponent self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();
            Unit fishUnit = SkillHelper.GetTrackFishUnit(self.DomainScene(), trackFishUnitId);
            if (fishUnit == null)
            {
                self.SetNormalBullet();
                return;
            }

            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            self.SetTrackDirection(fishUnitComponent.AimPoint.Vector);
        }

        private static void SetTrackDirection(this BulletUnitComponent self, Vector3 trackScreenPos)
        {
            // 追踪鱼重新计算方向, 在逻辑层设置好追踪屏幕位置
            Unit bulletUnit = self.Parent as Unit;
            TransformComponent transformComponent = bulletUnit.GetComponent<TransformComponent>();
            ref Vector3 bulletPosition = ref transformComponent.Info.ScreenPos;
            ref Vector2 moveDirection = ref self.Info.MoveDirection;
            float moveDirectionX = trackScreenPos.x - bulletPosition.x;
            if (Mathf.Abs(moveDirectionX) > BulletConfig.TrackDirectionFix)
                moveDirection.x = moveDirectionX;

            float moveDirectionY = trackScreenPos.y - bulletPosition.y;
            if (Mathf.Abs(moveDirectionY) > BulletConfig.TrackDirectionFix)
                moveDirection.y = moveDirectionY;

            moveDirection.Normalize();
        }

        private static void SetNormalBullet(this BulletUnitComponent self)
        {
            Unit unit = self.Parent as Unit;
            var attributeComponent = unit.GetComponent<NumericComponent>();
            attributeComponent.Set(NumericType.TrackFishId, BulletConfig.DefaultTrackFishUnitId);
        }
    }
}