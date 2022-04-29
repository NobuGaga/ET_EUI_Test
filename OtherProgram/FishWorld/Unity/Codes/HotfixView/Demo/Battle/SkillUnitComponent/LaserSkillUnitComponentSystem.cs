using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    public class LaserSkillUnitComponentAwakeSystem : AwakeSystem<LaserSkillUnitComponent, Transform>
    {
        public override void Awake(LaserSkillUnitComponent self, Transform node)
        {
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
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
    [FriendClass(typeof(SkillUnit))]
    [FriendClass(typeof(GameObjectComponent))]
    [FriendClass(typeof(LaserSkillUnitComponent))]
    public static class LaserSkillUnitComponentSystem
    {
        public static void UpdateLaserSkill(this SkillUnit self)
        {
            BattleViewComponent.Instance.SkillShoot(self.Parent.Parent.Id, self);
            long trackFishUnitId = self.GetTrackFishUnitId();
            if (trackFishUnitId == ConstHelper.DefaultTrackFishUnitId)
            {
                self.MoveToRemovePoint();
                return;
            }

            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            var laserSkillUnitComponent = self.LaserSkillUnitComponent as LaserSkillUnitComponent;
            if (gameObjectComponent == null || laserSkillUnitComponent == null)
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