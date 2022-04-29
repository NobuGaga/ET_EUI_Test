using UnityEngine;
using ET.EventType;

namespace ET
{
    [FriendClass(typeof(GlobalComponent))]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(PlayerSkillComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(CannonComponent))]
    public static class BulletViewComponentSystem
    {
        private static bool IsCanSkillShoot(bool isShowError)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            ushort battleCode = battleLogicComponent.CheckSelfSkillShootState();
            bool isCanShoot = battleCode == BattleCodeConfig.Success;
            if (!isShowError)
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

        /// <summary> 使用技能射击 </summary>
        public static void SkillShoot(this BattleViewComponent self, long playerUnitId, SkillUnit skillUnit)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            var playerComponent = battleLogicComponent.ZoneScene.GetComponent<PlayerComponent>();
            bool isSelf = playerUnitId == playerComponent.MyId;

            if (skillUnit.SkillType == SkillType.Aim && isSelf)
            {
                Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
                var playerSkillComponent = selfPlayerUnit.PlayerSkillComponent;
                Unit fishUnit = SkillHelper.GetTrackFishUnit(playerSkillComponent.TrackFishUnitId);
                if (fishUnit == null)
                    fishUnit = battleLogicComponent.UnitComponent.GetMaxScoreFishUnit();

                if (fishUnit == null)
                    return;
                
                var fishUnitComponent = fishUnit.FishUnitComponent;
                Vector3 screenPosition = fishUnitComponent.ScreenInfo.AimPoint;
                if (IsCanSkillShoot(true))
                    self.CallLogicShootBullet(ref screenPosition.x, ref screenPosition.y, ref fishUnit.UnitId);
                else
                    RotateCannon(ref screenPosition.x, ref screenPosition.y, false);
            }
            else if (skillUnit.SkillType == SkillType.Laser)
            {
                var playerSkillComponent = skillUnit.Parent as PlayerSkillComponent;
                long trackFishUnitId = playerSkillComponent.TrackFishUnitId;
                Unit fishUnit = SkillHelper.GetTrackFishUnit(trackFishUnitId);
                if (fishUnit == null)
                    return;

                var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
                int seatId = fisheryComponent.GetSeatId(playerUnitId);
                var fishUnitComponent = fishUnit.FishUnitComponent;
                Vector3 screenPosition = fishUnitComponent.ScreenInfo.AimPoint;
                CannonShootHelper.PushPool(RotateCannon(ref seatId, ref screenPosition.x, ref screenPosition.y, false));

                if (!isSelf || !IsCanSkillShoot(true))
                    return;

                int cannonStack = 1;
                battleLogicComponent.Shoot_C2M_Bullet(screenPosition.x, screenPosition.y, cannonStack, trackFishUnitId);
            }
        }

        /// <summary> 输入触控操作 </summary>
        public static void InputTouch(this BattleViewComponent self, ref float touchPosX, ref float touchPosY)
        {
            var skillComponent = BattleLogicComponent.Instance.SkillComponent;
            if (skillComponent.IsControlSelfShoot())
            {
                RotateCannon(ref touchPosX, ref touchPosY, false);
                return;
            }

            if (IsCanSkillShoot(true))
            {
                long trackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
                self.CallLogicShootBullet(ref touchPosX, ref touchPosY, ref trackFishUnitId);
            }
            else
                RotateCannon(ref touchPosX, ref touchPosY, false);
        }

        private static void RotateCannon(ref float touchPosX, ref float touchPosY, bool isPlayAnimation)
        {
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            CannonShootHelper.PushPool(RotateCannon(ref selfSeatId, ref touchPosX, ref touchPosY, isPlayAnimation));
        }

        private static CannonShootInfo RotateCannon(ref int seatId, ref float touchPosX, ref float touchPosY, bool isPlayAnimation)
        {
            Unit playerUnit = FisheryHelper.GetPlayerUnit(seatId);
            var cannonComponent = playerUnit.GetComponent<CannonComponent>();
            if (isPlayAnimation)
                cannonComponent.PlayAnimation();

            var gameObjectComponent = cannonComponent.Cannon.GameObjectComponent as GameObjectComponent;
            var cannonTransform = gameObjectComponent.GameObject.transform;
            var collector = cannonTransform.gameObject.GetComponent<ReferenceCollector>();

            // 在视图层获取屏幕坐标, 不放到 Mono 层因为 Mono 层的 Helper 类不能调用 Unity 的东西
            // 因为 Robot 工程有引用, 所以视图层的赋值操作需要在这里进行
            // 先通过当前炮台位置跟目标点屏幕坐标计算射击方向
            // 再通过射击方向计算炮台旋转方向
            // 最后使用旋转完后的炮台射击点位置来设置子弹的初始位置
            CannonShootInfo cannonShootInfo = CannonShootHelper.PopInfo();
            Camera camera = GlobalComponent.Instance.RawCannonCamera;
            Vector3 cannonScreenPos = camera.WorldToScreenPoint(cannonTransform.position);
            CannonShootHelper.SetLocalQuaternion(cannonShootInfo, touchPosX - cannonScreenPos.x,
                                                                  touchPosY - cannonScreenPos.y);
            cannonTransform.localRotation = cannonShootInfo.LocalRotation;
            var shootPointTransform = collector.Get<GameObject>("shoot_point").transform;
            Vector3 shootScreenPos = camera.WorldToScreenPoint(shootPointTransform.position);
            CannonShootHelper.InitInfo(cannonShootInfo, shootScreenPos);

            // 返回值方便发射子弹获取炮台信息
            return cannonShootInfo;
        }

        private static void CallLogicShootBullet(this BattleViewComponent self, ref float touchPosX,
                                                 ref float touchPosY, ref long trackFishUnitId)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            var bulletLogicComponent = battleLogicComponent.BulletLogicComponent;
            UnitInfo unitInfo = bulletLogicComponent.PopUnitInfo(selfSeatId, trackFishUnitId);
            self.CallLogicShootBullet(unitInfo, ref touchPosX, ref touchPosY);

            unitInfo.TryGetTrackFishUnitId(out trackFishUnitId);

            // Battle TODO 自己发射子弹发送协议, 后面把整个触控交互逻辑判断放到逻辑层
            // Battle TODO 炮倍, 暂时写死
            int cannonStack = 1;
            BattleLogicComponent.Instance.C2M_Fire(unitInfo.UnitId, touchPosX, touchPosY, cannonStack, trackFishUnitId);
        }

        /// <summary> 通知战斗逻辑发射子弹, 这里做一些视图层的处理然后再把数据传到逻辑层 </summary>
        public static void CallLogicShootBullet(this BattleViewComponent self, UnitInfo unitInfo,
                                                ref float touchPosX, ref float touchPosY)
        {
            int seatId = unitInfo.GetSeatId();

            // 需要在视图层初始化炮台信息后传到逻辑层, 再由逻辑层使用数据
            CannonShootInfo cannonShootInfo = RotateCannon(ref seatId, ref touchPosX, ref touchPosY, true);
            BattleLogicComponent.Instance.ShootBullet(unitInfo, cannonShootInfo);
        }
    }

    public class AfterShoot_BattleViewComponent : AEventClass<ReceiveFire>
    {
        protected override void Run(object obj)
        {
            var args = obj as ReceiveFire;
            BattleViewComponent.Instance.CallLogicShootBullet(args.UnitInfo, ref args.TouchPosX, ref args.TouchPosY);
        }
    }
}