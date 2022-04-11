using System.Collections.Generic;
using UnityEngine;
using ET.EventType;

namespace ET
{
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
            BattleLogic.Clear();

            // Battle Warning 需要保证 BattleLogicComponent 跟 BattleViewComponent 挂在同一个 Scene 上
            // 都通过 BattleTestConfig 的标记进行判断
            var battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();

            // Battle TODO 先更新鱼的位置, 再更新子弹的位置(因为子弹需要鱼的位置计算追踪)
            UpdateSkillBeforeFish(battleLogicComponent);
            UpdateFishUnitList(battleLogicComponent);
            UpdateSkillBeforeBullet(battleLogicComponent);
            UpdateBulletUnitList(battleLogicComponent);
        }

        private void UpdateSkillBeforeFish(BattleLogicComponent battleLogicComponent)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            if (currentScene != null)
                currentScene.GetComponent<SkillComponent>().FixedUpdateBeforeFish();
        }

        private void UpdateSkillBeforeBullet(BattleLogicComponent battleLogicComponent)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            if (currentScene != null)
                currentScene.GetComponent<SkillComponent>().UpdateBeforeBullet();
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
                if (!IsCanCollide(bulletUnit, fishUnit))
                    continue;

                var colliderViewComponent = fishUnit.GetComponent<ColliderViewComponent>();
                if (colliderViewComponent == null)
                    continue;

                var colliderMonoComponent = colliderViewComponent.MonoComponent;
                if (bulletColliderMonoCom.IsCollide(colliderMonoComponent))
                {
                    var bulletUnitCom = bulletUnit.GetComponent<BulletUnitComponent>();
                    Vector3 screenPosition;
                    if (bulletUnitCom.IsTrackBullet())
                    {
                        var fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
                        screenPosition = fishUnitComponent.AimPointPosition;
                    }
                    else
                        screenPosition = bulletColliderMonoCom.GetBulletCollidePoint();
                    battleLogicComponent.Collide(screenPosition.x, screenPosition.y, bulletUnit.Id, fishUnit.Id);
                    return;
                }
            }
        }

        private bool IsCanCollide(Unit bulletUnit, Unit fishUnit)
        {
            var bulletUnitComponent = bulletUnit.GetComponent<BulletUnitComponent>();
            long trackFishUnitId = bulletUnitComponent.GetTrackFishUnitId();
            return trackFishUnitId == BulletConfig.DefaultTrackFishUnitId || trackFishUnitId == fishUnit.Id;
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

    public static class BattleViewComponentSystem
    {
        /// <summary> 加快获取 CurrentScene 效率, 同 BattleLogicComponent 实现一样 </summary>
        public static Scene CurrentScene(this BattleViewComponent self)
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
    }

    // 先按 ZoneScene 的生命周期走, 后面看设计合理性是挂在 Current 还是 Zone
    // Event 的执行不依赖顺序
    // 目前订阅了 AfterCreateZoneScene 事件的处理只有 AfterCreateZoneScene_AddComponent
    // 避免别的事件执行顺序的依赖, 战斗相关组件释放要做好解耦断引用
    public class AfterCreateZoneScene_BattleViewComponent : AEvent<AfterCreateZoneScene>
    {
        protected override void Run(AfterCreateZoneScene args)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                args.ZoneScene.AddComponent<BattleViewComponent>();
        }
    }

    public class AfterCreateCurrentScene_BattleViewComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args)
        {
            if (BattleTestConfig.IsAddBattleToCurrent)
                args.CurrentScene.AddComponent<BattleViewComponent>();
        }
    }
}