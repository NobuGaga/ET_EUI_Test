using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(Unit))]
    public class ColliderViewComponentAwakeSystem : AwakeSystem<ColliderViewComponent>
    {
        public override void Awake(ColliderViewComponent self)
        {
            Unit unit = self.Parent as Unit;
            var gameObjectComponent = unit.GameObjectComponent as GameObjectComponent;
            int colliderId = unit.Type == UnitType.Bullet ? BulletConfig.BulletColliderID
                                                          : unit.ConfigId;
            self.ColliderMonoComponent = ColliderHelper.GetColliderComponent(colliderId,
                                                        gameObjectComponent.GameObject);
        }
    } 

    [ObjectSystem]
    public class ColliderViewComponentDestroySystem : DestroySystem<ColliderViewComponent>
    {
        public override void Destroy(ColliderViewComponent self) =>
                             self.ColliderMonoComponent = null;
    }

    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    [FriendClass(typeof(ColliderViewComponent))]
    public static class ColliderViewComponentSystem
    {
        public static void Update(this ColliderViewComponent self, Unit unit)
        {
            if (unit.Type == UnitType.Bullet)
            {
                var bulletUnitComponent = unit.BulletUnitComponent;
                BulletMoveInfo info = bulletUnitComponent.Info;
                self.ColliderMonoComponent.SetMoveDirection(info.MoveDirection);
            }

            self.ColliderMonoComponent.UpdateColliderCenter();

            if (unit.Type == UnitType.Fish)
            {
                FishUnitComponent fishUnitComponent = unit.FishUnitComponent;
                fishUnitComponent.AimPoint.Vector = self.ColliderMonoComponent.GetFishAimPoint();
                Vector3 aimPointPos = fishUnitComponent.AimPoint.Vector;
                fishUnitComponent.IsInScreen = aimPointPos.x > 0 && aimPointPos.y > 0 &&
                                               aimPointPos.x < Screen.width &&
                                               aimPointPos.y < Screen.height;
            }
        }
    }
}