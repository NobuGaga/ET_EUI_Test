using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class ColliderViewComponentAwakeSystem : AwakeSystem<ColliderViewComponent>
    {
        public override void Awake(ColliderViewComponent self)
        {
            Unit unit = self.Parent as Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            int colliderId = unit.UnitType == UnitType.Bullet ? BulletConfig.BulletColliderID : unit.ConfigId;
            self.MonoComponent = ColliderHelper.GetColliderComponent(colliderId,
                                                                     gameObjectComponent.GameObject);
        }
    } 

    [ObjectSystem]
    public class ColliderViewComponentDestroySystem : DestroySystem<ColliderViewComponent>
    {
        public override void Destroy(ColliderViewComponent self) =>
                             self.MonoComponent = null;
    }

    public static class ColliderViewComponentSystem
    {
        public static void Update(this ColliderViewComponent self)
        {
            Unit unit = self.Parent as Unit;

            if (unit.UnitType == UnitType.Bullet)
            {
                BulletUnitComponent bulletUnit = unit.GetComponent<BulletUnitComponent>();
                BulletMoveInfo info = bulletUnit.Info;
                self.MonoComponent.SetMoveDirection(info.MoveDirection);
            }

            self.MonoComponent.UpdateColliderCenter();

            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    TransformComponent transformComponent = unit.GetComponent<TransformComponent>();
                    FishUnitComponent fishUnitComponent = unit.GetComponent<FishUnitComponent>();
                    fishUnitComponent.AimPoint.Vector = self.MonoComponent.GetFishAimPoint();
                    ref Vector3 aimPointPos = ref fishUnitComponent.AimPoint.Vector;
                    transformComponent.IsInScreen = aimPointPos.x > 0 && aimPointPos.y > 0 &&
                                aimPointPos.x < Screen.width && aimPointPos.y < Screen.height;
                    break;
            }
        }
    }
}