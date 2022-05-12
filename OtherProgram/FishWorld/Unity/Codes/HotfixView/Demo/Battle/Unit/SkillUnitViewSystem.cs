using System;
using UnityEngine;
using ET.EventType;

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(SkillUnit))]
    [FriendClass(typeof(PlayerSkillComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    [FriendClass(typeof(UIFisheriesLowerEffectComponent))]
    public static class SkillUnitViewSystem
    {
        public static void AddGameObjectComponent(this SkillUnit self, string assetBundlePath, Transform node)
        {
            node.SetParent(self.GetNodeParent());
            node.localScale = Vector3.one;

            bool isUseModelPool = BattleConfig.IsUseModelPool;
            self.GameObjectComponent = self.AddComponent<GameObjectComponent, string, Transform>
                                                        (assetBundlePath, node, isUseModelPool);

            self.MoveToRemovePoint();
            if (self.SkillType == SkillType.Laser)
                self.LaserSkillUnitComponent = self.AddComponent<LaserSkillUnitComponent, Transform>(node, isUseModelPool);
        }

        private static UIFisheriesLowerEffectComponent GetUIParentComponent()
        {
            Scene zoneScene = BattleLogicComponent.Instance.ZoneScene;
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
                    return GetUIParentComponent().go_center;
            }
        }

        public static void MoveToRemovePoint(this SkillUnit self)
        {
            var gameObjectComponent = self.GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            (gameObjectComponent as GameObjectComponent).Transform.localPosition = SkillConfig.RemovePoint;
        }

        public static long GetTrackFishUnitId(this SkillUnit self)
        {
            var playerSkillLogicComponent = self.Parent as PlayerSkillComponent;
            return playerSkillLogicComponent.TrackFishUnitId;
        }

        public static Vector3 GetTrackPosition(this SkillUnit self)
        {
            Unit fishUnit = SkillHelper.GetTrackFishUnit(self.GetTrackFishUnitId());
            if (fishUnit == null)
                return SkillConfig.RemovePoint;

            return fishUnit.FishUnitComponent.ScreenInfo.AimPoint;
        }

        public static ObjectPoolComponent GetObjectPoolComponent(this SkillUnit self)
        {
            switch (self.SkillType)
            {
                case SkillType.Aim:
                    var uiComponent = GetUIParentComponent();
                    if (uiComponent != null)
                        return uiComponent.GetComponent<ObjectPoolComponent>();
                    return null;
                case SkillType.Laser:
                    var battleViewComponent = BattleViewComponent.Instance;
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
            if (trackFishUnitId == ConstHelper.DefaultTrackFishUnitId)
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
            var gameObjectComponent = self.GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            (gameObjectComponent as GameObjectComponent).Transform.localPosition = uiPos;
        }
    }

    public class SkillUnit_AfterCreateSkillUnit : AEvent<AfterCreateSkillUnit>
    {
        protected override void Run(AfterCreateSkillUnit args)
        {
            // Battle Warning 冰冻技能特效暂时在 UI 那边创建
            if (args.SkillUnit.SkillType == SkillType.Ice)
                return;

            InitModel(args.SkillUnit).Coroutine();
        }

        /// <summary>
        /// 初始化加载技能预设模型
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值, 这里异步加载实例化完后逻辑交由
        /// </summary>
        private async ETTask InitModel(SkillUnit unit)
        {
            var ret = TryGetAssetPathAndName(unit);
            string assetBundlePath = ret.Item1;
            string assetName = ret.Item2;

            var objectPoolComponent = unit.GetObjectPoolComponent();
            GameObject gameObject = objectPoolComponent?.PopObject(assetBundlePath);

            if (gameObject != null)
            {
                unit.AddGameObjectComponent(assetBundlePath, gameObject.transform);
                return;
            }

            gameObject = await ObjectInstantiateHelper.LoadAsset(assetBundlePath, assetName) as GameObject;
            if (gameObject == null || unit == null || unit.IsDisposed)
                return;

            gameObject = UnityEngine.Object.Instantiate(gameObject);
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