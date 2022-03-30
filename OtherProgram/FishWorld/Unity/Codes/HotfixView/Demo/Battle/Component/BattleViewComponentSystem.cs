using System.Collections.Generic;
using UnityEngine;
using ET.EventType;

namespace ET
{
    #region Event

    // 先按 ZoneScene 的生命周期走, 后面看设计合理性是挂在 Current 还是 Zone
    // Event 的执行不依赖顺序
    // 目前订阅了 AfterCreateZoneScene 事件的处理只有 AfterCreateZoneScene_AddComponent
    // 避免别的事件执行顺序的依赖, 战斗相关组件释放要做好解耦断引用
    public class AfterCreateZoneScene_BattleViewInit : AEvent<AfterCreateZoneScene>
    {
        protected override async ETTask Run(AfterCreateZoneScene args)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                args.ZoneScene.AddComponent<BattleViewComponent>();

            await ETTask.CompletedTask;
        }
    }

    public class AfterCreateCurrentScene_BattleViewInit : AEvent<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(AfterCreateCurrentScene args)
        {
            if (BattleTestConfig.IsAddBattleToCurrent)
                args.CurrentScene.AddComponent<BattleViewComponent>();

            await ETTask.CompletedTask;
        }
    }

    public class AfterShoot_BattleViewComponent : AEvent<ReceiveFire>
    {
        protected override async ETTask Run(ReceiveFire args)
        {
            BattleViewComponent battleViewCom = args.CurrentScene.GetBattleViewComponent();
            battleViewCom.CallLogicShootBullet(args.UnitInfo, ref args.TouchPosX, ref args.TouchPosY);
            await ETTask.CompletedTask;
        }
    }

    #endregion

    #region Life Circle

    [ObjectSystem]
    public class BattleViewComponentAwakeSystem : AwakeSystem<BattleViewComponent>
    {
        public override void Awake(BattleViewComponent self)
        {
            // Mono 层方法只能在 View 层调用
            BattleLogic.Init();

            // 战斗用另外一个对象池组件, 生命周期跟战斗视图组件
            // 只用来管理鱼跟子弹, 不直接 Add 到 Scene 上是因为 Scene 上面可能已经 Add 了
            self.AddComponent<ObjectPoolComponent>();
        }
    }

    [ObjectSystem]
    public class BattleViewComponentUpdateSystem : UpdateSystem<BattleViewComponent>
    {
        public override void Update(BattleViewComponent self)
        {
            // Battle Warning 需要保证 BattleLogicComponent 跟 BattleViewComponent 挂在同一个 Scene 上
            // 都通过 BattleTestConfig 的标记进行判断
            var battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            // Battle TODO 先更新鱼的位置, 再更新子弹的位置(因为子弹需要鱼的位置计算追踪)
            UpdateFishUnitList(battleLogicComponent);
            UpdateBulletUnitList(battleLogicComponent);
        }

        private void UpdateFishUnitList(BattleLogicComponent battleLogicComponent)
        {
            HashSet<Unit> fishUnitList = GetFishUnitList(battleLogicComponent);
            if (fishUnitList == null)
                return;

            foreach (Unit fishUnit in fishUnitList)
            {
                if (!fishUnit.GetComponent<BattleUnitLogicComponent>().IsUpdate)
                    continue;

                fishUnit.FixedUpdate();
                fishUnit.Update();
            }
        }

        private void UpdateBulletUnitList(BattleLogicComponent battleLogicComponent)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            if (currentScene == null)
                return;

            BulletLogicComponent bulletLogicComponent = currentScene.GetComponent<BulletLogicComponent>();
            List<long> bulletIdList = bulletLogicComponent.BulletIdList;
            long bulletUnitId;
            Unit bulletUnit;
            for (int index = bulletIdList.Count - 1; index >= 0; index--)
            {
                bulletUnitId = bulletIdList[index];
                bulletUnit = bulletLogicComponent.GetChild<Unit>(bulletUnitId);
                if (bulletUnit == null)
                {
                    Log.Error($"private BattleViewComponent.UpdateBulletUnitList bullet is not exist. bulletUnitId = { bulletUnitId }");
                    continue;
                }

                bulletUnit.FixedUpdate();
                bulletUnit.Update();
                CheckBulletCollide(battleLogicComponent, bulletUnit);
            }
        }

        private void CheckBulletCollide(BattleLogicComponent battleLogicComponent, Unit bulletUnit)
        {
            HashSet<Unit> fishUnitList = GetFishUnitList(battleLogicComponent);
            if (fishUnitList == null)
                return;

            var bulletColliderViewCom = bulletUnit.GetComponent<ColliderViewComponent>();
            if (bulletColliderViewCom == null)
                return;

            var bulletColliderMonoCom = bulletColliderViewCom.MonoComponent;
            foreach (Unit fishUnit in fishUnitList)
            {
                var colliderViewComponent = fishUnit.GetComponent<ColliderViewComponent>();
                if (colliderViewComponent == null)
                    continue;

                var colliderMonoComponent = colliderViewComponent.MonoComponent;
                if (bulletColliderMonoCom.IsCollide(colliderMonoComponent))
                {
                    Vector3 screenPosition = bulletUnit.GetScreenPos();
                    battleLogicComponent.Collide(screenPosition.x, screenPosition.y, bulletUnit.Id, fishUnit.Id);
                    return;
                }
            }
        }

        private HashSet<Unit> GetFishUnitList(BattleLogicComponent battleLogicComponent)
        {
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            if (unitComponent == null)
                return null;

            return unitComponent.GetFishUnitList();
        }
    }

    [ObjectSystem]
    public class BattleViewComponentDestroySystem : DestroySystem<BattleViewComponent>
    {
        public override void Destroy(BattleViewComponent self)
        {
            // Battle TODO
        }
    }

    #endregion

    #region Base Function

    public static class BattleViewComponentSystem
    {
        /// <summary> 加快获取 CurrentScene 效率, 同 BattleLogicComponent 实现一样 </summary>
        private static Scene CurrentScene(this BattleViewComponent self)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                // CurrentScene 全局只有一个
                return self.ZoneScene().CurrentScene();

            return self.DomainScene();
        }

        /// <summary> 拓展实现 Scene 方法, 方便获取 BattleViewComponent </summary>
        public static BattleViewComponent GetBattleViewComponent(this Scene scene)
        {
            if (BattleTestConfig.IsAddBattleToZone && scene.SceneType == SceneType.Current)
                scene = scene.ZoneScene();

            return scene.GetComponent<BattleViewComponent>();
        }

        public static void RotateCannon(this BattleViewComponent self, ref float touchPosX, ref float touchPosY)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            self.RotateCannon(selfSeatId, ref touchPosX, ref touchPosY);
        }

        private static CannonShootInfo RotateCannon(this BattleViewComponent self, int seatId, ref float touchPosX, 
                                                                                               ref float touchPosY)
        {
            Scene currentScene = self.CurrentScene();
            Unit playerUnit = UnitHelper.GetPlayUnitBySeatId(currentScene, seatId);
            CannonComponent cannonComponent = playerUnit.GetComponent<CannonComponent>();
            GameObjectComponent gameObjectComponent = cannonComponent.Cannon.GetComponent<GameObjectComponent>();
            Transform transform = gameObjectComponent.GameObject.transform;

            // 在视图层获取屏幕坐标, 不放到 Mono 层因为 Mono 层的 Helper 类不能调用 Unity 的东西
            Vector3 shootScreenPos = GlobalComponent.Instance.CannonSeatLayout.GetShootScreenPoint(seatId);
            Vector2 shootDirection = new Vector2(touchPosX - shootScreenPos.x, touchPosY - shootScreenPos.y);

            CannonShootInfo cannonShootInfo = CannonShootHelper.PopInfo();
            CannonShootHelper.InitInfo(cannonShootInfo, transform.localRotation, shootDirection, shootScreenPos);
            transform.localRotation = cannonShootInfo.LocalRotation;

            // 返回值方便发射子弹获取炮台信息
            return cannonShootInfo;
        }

        public static void CallLogicShootBullet(this BattleViewComponent self, ref float touchPosX, ref float touchPosY)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();

            UnitInfo unitInfo = self.CallLogicShootBullet(selfSeatId, ref touchPosX, ref touchPosY);
            
            // Battle TODO 自己发射子弹发送协议, 后面把整个触控交互逻辑判断放到逻辑层
            // Battle TODO 炮倍, 暂时写死
            int cannonStack = 1;
            unitInfo.TryGetTrackFishUnitId(out long trackFishUnitId);
            BattleLogicComponent battleLogicCom = currentScene.GetBattleLogicComponent();
            battleLogicCom.C2M_Fire(unitInfo.UnitId, touchPosX, touchPosY, cannonStack, trackFishUnitId);
        }

        private static UnitInfo CallLogicShootBullet(this BattleViewComponent self, int seatId,
                                                ref float touchPosX, ref float touchPosY)
        {
            Scene currentScene = self.CurrentScene();
            BulletLogicComponent bulletLogicComponent = currentScene.GetComponent<BulletLogicComponent>();
            UnitInfo unitInfo = bulletLogicComponent.PopUnitInfo(seatId);
            self.CallLogicShootBullet(unitInfo, ref touchPosX, ref touchPosY);
            return unitInfo;
        }

        /// <summary> 通知战斗逻辑发射子弹, 这里做一些视图层的处理然后再把数据传到逻辑层 </summary>
        public static void CallLogicShootBullet(this BattleViewComponent self, UnitInfo unitInfo,
                                                ref float touchPosX, ref float touchPosY)
        {
            int seatId = unitInfo.GetSeatId();

            // 需要在视图层初始化炮台信息后传到逻辑层, 再由逻辑层使用数据
            CannonShootInfo cannonShootInfo = self.RotateCannon(seatId, ref touchPosX, ref touchPosY);

            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            battleLogicComponent.ShootBullet(unitInfo, cannonShootInfo);
        }
    }

    #endregion
}