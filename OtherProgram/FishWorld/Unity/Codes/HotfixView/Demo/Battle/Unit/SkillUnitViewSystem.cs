using System;
using UnityEngine;
using ET.EventType;

namespace ET
{
    public static class SkillUnitViewSystem
    {
        public static void AddGameObjectComponent(this SkillUnit self, string assetBundlePath, Transform node)
        {
            node.SetParent(self.GetNodeParent());
            node.localScale = Vector3.one;

            bool isUseModelPool = BattleConfig.IsUseModelPool;
            self.AddComponent<GameObjectComponent, string, Transform>(assetBundlePath, node, isUseModelPool);

            self.MoveToRemovePoint();

            if (self.SkillType == SkillType.Laser)
                self.AddComponent<LaserSkillUnitComponent, Transform>(node, isUseModelPool);
        }

        private static UIFisheriesLowerEffectComponent GetUIParentComponent(this SkillUnit self)
        {
            Scene zoneScene = self.DomainScene().ZoneScene();
            if (zoneScene == null)
                return null;

            UI ui = UIHelper.Get(zoneScene, UIType.UIFisheriesLowerEffect);
            return ui.GetComponent<UIFisheriesLowerEffectComponent>();
        }

        private static Transform GetNodeParent(this SkillUnit self)
        {
            switch (self.SkillType)
            {
                case SkillType.Laser:
                    return ReferenceHelper.BulletRootNode.transform;
                default:
                    return self.GetUIParentComponent().go_center;
            }
        }

        public static void MoveToRemovePoint(this SkillUnit self)
        {
            var gameObjectComponent = self.GetComponent<GameObjectComponent>();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localPosition = SkillConfig.RemovePoint;
        }

        public static long GetTrackFishUnitId(this SkillUnit self)
        {
            var playerSkillLogicComponent = self.Parent as PlayerSkillComponent;
            return playerSkillLogicComponent.TrackFishUnitId;
        }

        public static Vector3 GetTrackPosition(this SkillUnit self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();
            Unit fishUnit = SkillHelper.GetTrackFishUnit(self.DomainScene(), trackFishUnitId);
            if (fishUnit == null)
                return SkillConfig.RemovePoint;

            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            return fishUnitComponent.AimPointPosition;
        }

        public static ObjectPoolComponent GetObjectPoolComponent(this SkillUnit self)
        {
            switch (self.SkillType)
            {
                case SkillType.Aim:
                    var uiComponent = self.GetUIParentComponent();
                    if (uiComponent != null)
                        return uiComponent.GetComponent<ObjectPoolComponent>();
                    return null;
                case SkillType.Laser:
                    var battleViewComponent = self.DomainScene().GetBattleViewComponent();
                    return battleViewComponent.GetComponent<ObjectPoolComponent>();
                default:
                    return null;
            }
        }

        public static void UpdateBeforeBullet(this SkillUnit self)
        {
            switch (self.SkillType)
            {
                case SkillType.Aim:
                    self.UpdateAimSkill();
                    return;
                case SkillType.Laser:
                    self.UpdateLaserSkill();
                    return;
            }
        }

        public static void UpdatePosition(this SkillUnit self)
        {
            long trackFishUnitId = self.GetTrackFishUnitId();
            if (trackFishUnitId == BulletConfig.DefaultTrackFishUnitId)
            {
                self.MoveToRemovePoint();
                return;
            }

            Vector3 trackPosition = self.GetTrackPosition();
            if (trackPosition == SkillConfig.RemovePoint)
            {
                self.MoveToRemovePoint();
                return;
            }

            Vector3 uiPos = UIHelper.ScreenPosToUI(trackPosition.x, trackPosition.y);
            var gameObjectComponent = self.GetComponent<GameObjectComponent>();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localPosition = uiPos;
        }
    }

    public class SkillUnit_AfterCreateSkillUnit : AEvent<AfterCreateSkillUnit>
    {
        protected override void Run(AfterCreateSkillUnit args)
        {
            // Battle Warning 冰冻技能特效暂时在 UI 那边创建
            if (args.SkillUnit.SkillType == SkillType.Ice)
                return;

            InitModel(args.CurrentScene, args.SkillUnit).Coroutine();
        }

        /// <summary>
        /// 初始化加载技能预设模型
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值, 这里异步加载实例化完后逻辑交由
        /// </summary>
        private async ETTask InitModel(Scene currentScene, SkillUnit unit)
        {
            var ret = TryGetAssetPathAndName(unit);
            string assetBundlePath = ret.Item1;
            string assetName = ret.Item2;

            ObjectPoolComponent objectPoolComponent = unit.GetObjectPoolComponent();
            GameObject gameObject = objectPoolComponent?.PopObject(assetBundlePath);

            // 这里会抛出异常
            if (gameObject == null)
                gameObject = await ObjectInstantiateHelper.InitModel(currentScene, unit, assetBundlePath, assetName);

            // Unit 为 null 则已经被释放掉 
            if (gameObject != null)
                unit.AddGameObjectComponent(assetBundlePath, gameObject.transform);
        }

        private ValueTuple<string, string> TryGetAssetPathAndName(SkillUnit unit)
        {
            string assetBundlePath = string.Empty;
            string assetName = string.Empty;

            switch (unit.SkillType)
            {
                case SkillType.Aim:
                    assetBundlePath = ABPath.eff_skill_suoding_aimAB;
                    assetName = "eff_skill_suoding_aim";
                    break;
                case SkillType.Laser:
                    assetBundlePath = ABPath.WildHitAB;
                    assetName = "1";
                    break;
            }

            return new ValueTuple<string, string>(assetBundlePath, assetName);
        }
    }
}