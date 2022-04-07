using System;
using UnityEngine;
using ET.EventType;
using System.Collections.Generic;

namespace ET
{
    public static class SkillUnitViewSystem
    {
        public static void Update(this SkillUnit self)
        {
            int skillType = self.SkillType;
            if (skillType == SkillType.Ice)
                return;

            if (skillType == SkillType.Aim)
                self.UpdateAimSkill();
            else if (skillType == SkillType.Laser)
            {
                // Battle TODO
            }
        }

        private static void UpdateAimSkill(this SkillUnit self)
        {
            var bulletLogicComponent = self.DomainScene().GetComponent<BulletLogicComponent>();
            List<long> bulletIdList = bulletLogicComponent.BulletIdList;
            if (bulletIdList.Count == 0)
            {
                self.SetPosition(SkillConfig.RemovePoint);
                return;
            }

            long trackFishUnitId = BulletConfig.DefaultTrackFishUnitId;
            for (ushort index = 0; index < bulletIdList.Count; index++)
            {
                long bulletUnitId = bulletIdList[index];
                Unit bulletUnit = bulletLogicComponent.GetChild<Unit>(bulletUnitId);
                var bulletUnitComponent = bulletUnit.GetComponent<BulletUnitComponent>();
                trackFishUnitId = bulletUnitComponent.GetTrackFishUnitId();
                if (trackFishUnitId != BulletConfig.DefaultTrackFishUnitId)
                    break;
            }

            var battleLogicComponent = self.DomainScene().GetBattleLogicComponent();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            Unit fishUnit = unitComponent.Get(trackFishUnitId);
            self.SetPosition(fishUnit.GetScreenPosition());
        }

        private static void SetPosition(this SkillUnit unit, Vector3 screenPosition)
        {
            Log.Debug($"SetPosition screenPosition = { screenPosition }");
            Vector3 uiPos = UIHelper.ScreenPosToUI(screenPosition.x, screenPosition.y);
            Log.Debug($"SetPosition uiPos = { uiPos }");
            
            var gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            gameObjectComponent.Transform.localPosition = uiPos;
        }
    }

    public class SkillUnit_AfterCreateSkillUnit : AEvent<AfterCreateSkillUnit>
    {
        protected override void Run(AfterCreateSkillUnit args) =>
                                InitModel(args.CurrentScene, args.SkillUnit).Coroutine();

        /// <summary>
        /// 初始化加载技能预设模型
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值, 这里异步加载实例化完后逻辑交由
        /// </summary>
        private async ETTask InitModel(Scene currentScene, SkillUnit unit)
        {
            var ret = TryGetAssetPathAndName(unit);
            string assetBundlePath = ret.Item1;
            string assetName = ret.Item2;

            // 这里会抛出异常
            GameObject gameObject = await ObjectInstantiateHelper.InitModel(currentScene, unit, assetBundlePath, assetName);

            // Unit 已经被释放掉 
            if (gameObject == null)
                return;

            UI ui = UIHelper.Get(currentScene.ZoneScene(), UIType.UIFisheriesLowerEffect);
            var uiComponent = ui.GetComponent<UIFisheriesLowerEffectComponent>();

            Transform node = gameObject.transform;
            node.SetParent(uiComponent.go_center);
            node.localPosition = Vector3.zero;

            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            unit.AddComponent<GameObjectComponent, string, Transform>(assetBundlePath, node, isUseModelPool);
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

                    break;
            }

            return new ValueTuple<string, string>(assetBundlePath, assetName);
        }
    }
}