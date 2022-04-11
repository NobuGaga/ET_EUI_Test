using UnityEngine;
using ET.EventType;

namespace ET
{
    public static class BulletViewComponentSystem
    {
        /// <summary> 使用技能射击 </summary>
        public static void SkillShoot(this BattleViewComponent self, long playerUnitId, SkillUnit skillUnit)
        {
            PlayerComponent playerComponent = self.ZoneScene().GetComponent<PlayerComponent>();
            bool isSelf = playerUnitId == playerComponent.MyId;

            if (skillUnit.SkillType == SkillType.Aim && isSelf)
            {
                if (self.IsCanSkillShoot(true))
                    self.CallLogicShootBullet();
                else
                    self.RotateCannon(false);
            }
            else if (skillUnit.SkillType == SkillType.Laser)
            {
                Scene currentScene = self.CurrentScene();
                var playerSkillComponent = skillUnit.Parent as PlayerSkillComponent;
                long trackFishUnitId = playerSkillComponent.TrackFishUnitId;
                Unit fishUnit = SkillHelper.GetTrackFishUnit(currentScene, trackFishUnitId);
                if (fishUnit == null)
                    return;

                var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
                int seatId = fisheryComponent.GetSeatId(playerUnitId);
                FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
                Vector3 screenPosition = fishUnitComponent.AimPointPosition;
                self.RotateCannon(seatId, ref screenPosition.x, ref screenPosition.y, false);

                if (!isSelf || !self.IsCanSkillShoot(true))
                    return;

                var battleLogicComponent = currentScene.GetBattleLogicComponent();
                int cannonStack = 1;
                battleLogicComponent.Shoot_C2M_Bullet(screenPosition.x, screenPosition.y, cannonStack, trackFishUnitId);
            }
        }

        /// <summary> 输入触控操作 </summary>
        public static void InputTouch(this BattleViewComponent self, ref float touchPosX, ref float touchPosY)
        {
            if (self.IsCanNormalShoot(true))
                self.CallLogicShootBullet(ref touchPosX, ref touchPosY);
            else
                self.RotateCannon(ref touchPosX, ref touchPosY, false);
        }

        private static bool IsCanSkillShoot(this BattleViewComponent self, bool isHandle)
        {
            Scene battleScene = self.Parent as Scene;
            BattleLogicComponent battleLogicComponent = battleScene.GetComponent<BattleLogicComponent>();
            ushort battleCode = battleLogicComponent.CheckSelfSkillShootState();
            bool isCanShoot = battleCode == BattleCodeConfig.Success;
            if (!isHandle)
                return isCanShoot;

            switch (battleCode)
            {
                // Battle TODO 错误码后面改成读文本表
                case BattleCodeConfig.UpperLimitBullet:
                    Log.Error($"发射子弹超出配置表配置上限 : { BulletConfig.ShootMaxBulletCount }");
                    return isCanShoot;
                case BattleCodeConfig.NotEnoughMoney:
                    Log.Error("金币不足");
                    return isCanShoot;
            }

            return isCanShoot;
        }

        private static bool IsCanNormalShoot(this BattleViewComponent self, bool isHandle)
        {
            Scene currentScene = self.CurrentScene();
            SkillComponent skillComponent = currentScene.GetComponent<SkillComponent>();
            if (skillComponent.IsControlSelfShoot())
                return false;

            return self.IsCanSkillShoot(isHandle);
        }

        private static Unit GetMaxScoreFish(this BattleViewComponent self)
        {
            Scene currentScene = self.CurrentScene();
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            var playerSkillComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();
            Unit fishUnit = SkillHelper.GetTrackFishUnit(currentScene, playerSkillComponent.TrackFishUnitId);
            if (fishUnit != null)
                return fishUnit;

            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            return unitComponent.GetMaxScoreFish();
        }

        private static void RotateCannon(this BattleViewComponent self, bool isPlayAnimation)
        {
            Unit fishUnit = self.GetMaxScoreFish();
            if (fishUnit == null)
                return;

            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            Vector3 screenPosition = fishUnitComponent.AimPointPosition;
            self.RotateCannon(ref screenPosition.x, ref screenPosition.y, isPlayAnimation);
        }

        private static void RotateCannon(this BattleViewComponent self, ref float touchPosX, ref float touchPosY,
                                        bool isPlayAnimation)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            self.RotateCannon(selfSeatId, ref touchPosX, ref touchPosY, isPlayAnimation);
        }

        private static CannonShootInfo RotateCannon(this BattleViewComponent self, int seatId,
                                                    ref float touchPosX, ref float touchPosY, bool isPlayAnimation)
        {
            Scene currentScene = self.CurrentScene();
            Unit playerUnit = UnitHelper.GetPlayUnitBySeatId(currentScene, seatId);
            CannonComponent cannonComponent = playerUnit.GetComponent<CannonComponent>();
            if (isPlayAnimation)
                cannonComponent.PlayAnimation();

            GameObjectComponent gameObjectComponent = cannonComponent.Cannon.GetComponent<GameObjectComponent>();
            Transform cannonTrans = gameObjectComponent.GameObject.transform;
            ReferenceCollector collector = cannonTrans.gameObject.GetComponent<ReferenceCollector>();
            Transform shootPointTrans = collector.Get<GameObject>("shoot_point").transform;

            // 在视图层获取屏幕坐标, 不放到 Mono 层因为 Mono 层的 Helper 类不能调用 Unity 的东西
            // 因为 Robot 工程有引用, 所以视图层的赋值操作需要在这里进行
            // 先通过当前炮台位置跟目标点屏幕坐标计算射击方向
            // 再通过射击方向计算炮台旋转方向
            // 最后使用旋转完后的炮台射击点位置来设置子弹的初始位置
            CannonShootInfo cannonShootInfo = CannonShootHelper.PopInfo();
            Vector3 cannonScreenPos = GlobalComponent.Instance.CannonCamera.WorldToScreenPoint(cannonTrans.position);
            Vector2 shootDirection = new Vector2(touchPosX - cannonScreenPos.x, touchPosY - cannonScreenPos.y);
            CannonShootHelper.SetLocalQuaternion(cannonShootInfo, shootDirection);
            cannonTrans.localRotation = cannonShootInfo.LocalRotation;
            Vector3 shootScreenPos = GlobalComponent.Instance.CannonCamera.WorldToScreenPoint(shootPointTrans.position);
            CannonShootHelper.InitInfo(cannonShootInfo, shootScreenPos);

            // 返回值方便发射子弹获取炮台信息
            return cannonShootInfo;
        }

        private static void CallLogicShootBullet(this BattleViewComponent self)
        {
            Unit fishUnit = self.GetMaxScoreFish();
            if (fishUnit == null)
                return;

            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            Vector3 screenPosition = fishUnitComponent.AimPointPosition;
            long trackFishUnitId = fishUnit.Id;
            self.CallLogicShootBullet(ref screenPosition.x, ref screenPosition.y, ref trackFishUnitId);
        }

        private static void CallLogicShootBullet(this BattleViewComponent self, ref float touchPosX,
                                                ref float touchPosY)
        {
            long trackFishUnitId = BulletConfig.DefaultTrackFishUnitId;
            self.CallLogicShootBullet(ref touchPosX, ref touchPosY, ref trackFishUnitId);
        }

        private static void CallLogicShootBullet(this BattleViewComponent self, ref float touchPosX,
                                                ref float touchPosY, ref long trackFishUnitId)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            UnitInfo unitInfo = self.CallLogicShootBullet(selfSeatId, ref touchPosX, ref touchPosY, ref trackFishUnitId);
            int cannonStack = 1;
            unitInfo.TryGetTrackFishUnitId(out trackFishUnitId);
            BattleLogicComponent battleLogicComponent = currentScene.GetBattleLogicComponent();

            // Battle TODO 自己发射子弹发送协议, 后面把整个触控交互逻辑判断放到逻辑层
            // Battle TODO 炮倍, 暂时写死
            battleLogicComponent.C2M_Fire(unitInfo.UnitId, touchPosX, touchPosY, cannonStack, trackFishUnitId);
        }

        private static UnitInfo CallLogicShootBullet(this BattleViewComponent self, int seatId, ref float touchPosX,
                                                     ref float touchPosY, ref long trackFishUnitId)
        {
            Scene currentScene = self.CurrentScene();
            BulletLogicComponent bulletLogicComponent = currentScene.GetComponent<BulletLogicComponent>();
            UnitInfo unitInfo = bulletLogicComponent.PopUnitInfo(seatId, trackFishUnitId);
            self.CallLogicShootBullet(unitInfo, ref touchPosX, ref touchPosY);
            return unitInfo;
        }

        /// <summary> 通知战斗逻辑发射子弹, 这里做一些视图层的处理然后再把数据传到逻辑层 </summary>
        public static void CallLogicShootBullet(this BattleViewComponent self, UnitInfo unitInfo,
                                                ref float touchPosX, ref float touchPosY)
        {
            int seatId = unitInfo.GetSeatId();

            // 需要在视图层初始化炮台信息后传到逻辑层, 再由逻辑层使用数据
            CannonShootInfo cannonShootInfo = self.RotateCannon(seatId, ref touchPosX, ref touchPosY, true);

            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            battleLogicComponent.ShootBullet(unitInfo, cannonShootInfo);
        }
    }

    public class AfterShoot_BattleViewComponent : AEvent<ReceiveFire>
    {
        protected override void Run(ReceiveFire args)
        {
            BattleViewComponent battleViewComponent = args.CurrentScene.GetBattleViewComponent();
            battleViewComponent.CallLogicShootBullet(args.UnitInfo, ref args.TouchPosX, ref args.TouchPosY);
        }
    }
}