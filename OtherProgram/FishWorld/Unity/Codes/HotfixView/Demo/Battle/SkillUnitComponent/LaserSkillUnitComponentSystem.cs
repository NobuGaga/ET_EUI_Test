using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class LaserSkillUnitComponentAwakeSystem : AwakeSystem<LaserSkillUnitComponent, Transform>
    {
        public override void Awake(LaserSkillUnitComponent self, Transform node)
        {
            Scene currentScene = self.DomainScene();
            SkillUnit skillUnit = self.Parent as SkillUnit;
            PlayerSkillComponent playerSkillComponent = skillUnit.Parent as PlayerSkillComponent;
            Unit playerUnit = playerSkillComponent.Parent as Unit;
            var attributeComponent = playerUnit.GetComponent<NumericComponent>();
            int seatId = attributeComponent.GetAsInt(NumericType.Pos);

            self.CannonShootPointNode = CannonHelper.GetShootPointNode(currentScene, seatId);

            // 上层还有一个节点 = =
            Transform otherRootNode = node.GetChild(0);
            for (ushort index = 0; index < otherRootNode.childCount; index++)
            {
                Transform childNode = otherRootNode.GetChild(index);
                if (childNode.name != SkillConfig.LaserEndNodeName)
                    continue;

                self.EndNode = childNode;
                break;
            }

            skillUnit.UpdateLaserSkill();
        }
    }

    [ObjectSystem]
    public class LaserSkillUnitComponentDestroySystem : DestroySystem<LaserSkillUnitComponent>
    {
        public override void Destroy(LaserSkillUnitComponent self)
        {
            self.CannonShootPointNode = null;
            self.EndNode = null;
        }
    }

    [FriendClass(typeof(GlobalComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    [FriendClass(typeof(LaserSkillUnitComponent))]
    public static class LaserSkillUnitComponentSystem
    {
        public static void UpdateLaserSkill(this SkillUnit self)
        {
            var playerSkillLogicComponent = self.Parent as PlayerSkillComponent;
            Unit playerUnit = playerSkillLogicComponent.Parent as Unit;
            Scene currentScene = self.DomainScene();
            var battleViewComponent = BattleViewComponent.Instance;
            battleViewComponent.SkillShoot(playerUnit.Id, self);

            long trackFishUnitId = self.GetTrackFishUnitId();
            if (trackFishUnitId == ConstHelper.DefaultTrackFishUnitId)
            {
                self.MoveToRemovePoint();
                return;
            }

            var laserSkillUnitComponent = self.GetComponent<LaserSkillUnitComponent>();

            var gameObjectComponent = self.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            Camera cannonCamera = GlobalComponent.Instance.RawCannonCamera;
            Transform shootPointTrans = laserSkillUnitComponent.CannonShootPointNode;
            Vector3 shootScreenPos = cannonCamera.WorldToScreenPoint(shootPointTrans.position);
            gameObjectComponent.Transform.localPosition = CannonHelper.ScreenPointToCannonPosition(shootScreenPos);

            // 使用追踪鱼的屏幕坐标, 重新设置会炮台的 z 值(炮台的 z 值时相对炮台坐标系的)
            Vector3 screenPosition = self.GetTrackPosition();
            shootScreenPos.x = screenPosition.x;
            shootScreenPos.y = screenPosition.y;
            laserSkillUnitComponent.EndNode.position = cannonCamera.ScreenToWorldPoint(shootScreenPos);
        }
    }
}