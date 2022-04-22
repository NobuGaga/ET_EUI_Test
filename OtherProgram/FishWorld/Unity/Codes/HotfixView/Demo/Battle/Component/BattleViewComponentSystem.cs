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
            // 只用来管理鱼跟子弹, 不直接 Add 到 Scene 上是因为 CurrentScene 跟 ZoneScene 已经有了
            // 技能挂载在 UI 界面的节点由 UI 的 ObjectPoolComponent 管理
            // 挂载到非 UI 界面的节点由 BattleViewComponent 管理
            self.AddComponent<ObjectPoolComponent>();
        }
    }

    [ObjectSystem]
    public class BattleViewComponentDestroySystem : DestroySystem<BattleViewComponent>
    {
        public override void Destroy(BattleViewComponent self) => BattleLogic.Clear();
    }

    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BulletLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    [FriendClass(typeof(ColliderViewComponent))]
    public class BattleViewComponentUpdateSystem : UpdateSystem<BattleViewComponent>
    {
        public override void Update(BattleViewComponent self)
        {
            BattleLogic.Update();

            Scene currentScene = self.CurrentScene();
            if (currentScene == null)
                return;

            // Battle Warning 需要保证 BattleLogicComponent 跟 BattleViewComponent 挂在同一个 Scene 上
            // 都通过 BattleConfig 的标记进行判断
            var battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();

            // 有序更新, 统一在一个 Update 里进行
            currentScene?.GetComponent<SkillComponent>().FixedUpdateBeforeFish();
            UpdateFishUnitList(battleLogicComponent, unitComponent);
            currentScene?.GetComponent<SkillComponent>().UpdateBeforeBullet();
            UpdateBulletUnitList(self, currentScene, unitComponent);
        }

        private void UpdateFishUnitList(BattleLogicComponent battleLogicComponent, UnitComponent unitComponent)
        {
            // 帧更新开始将移除鱼列表清除
            // 在 FishUnitComponent 的 FixedUpdate 里进行增加
            var removeFishUnitIdList = battleLogicComponent.RemoveUnitIdList;
            removeFishUnitIdList.Clear();
            
            unitComponent.FixedUpdateFishUnitList();

            // 更新完数据后马上把无效数据删除
            for (var index = removeFishUnitIdList.Count - 1; index >= 0; index--)
                unitComponent.Remove(removeFishUnitIdList[index]);

            unitComponent.UpdateFishUnitList();
        }

        private void UpdateBulletUnitList(BattleViewComponent self, Scene currentScene, UnitComponent unitComponent)
        {
            var battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            BulletLogicComponent bulletLogicComponent = currentScene.GetComponent<BulletLogicComponent>();

            List<long> bulletIdList = bulletLogicComponent.BulletIdList;
            long bulletUnitId;
            Unit bulletUnit;

            for (var index = bulletIdList.Count - 1; index >= 0; index--)
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

                if (bulletUnit.ColliderViewComponent != null)
                    battleLogicComponent.Foreach(unitComponent.GetFishUnitList(), IsCollide, bulletUnit);
            }
        }

        /// <summary> 是否发生碰撞</summary>
        /// <returns>返回 true 则跳出循环</returns>
        private bool IsCollide(Unit fishUnit, Unit bulletUnit, BattleLogicComponent battleLogicComponent)
        {
            var bulletUnitComponent = bulletUnit.BulletUnitComponent;
            ref long trackFishUnitId = ref bulletUnitComponent.TrackFishUnitId;
            if (trackFishUnitId != BulletConfig.DefaultTrackFishUnitId && trackFishUnitId != fishUnit.Id)
                return true;

            var colliderViewComponent = fishUnit.ColliderViewComponent as ColliderViewComponent;
            if (colliderViewComponent == null)
                return true;

            var bulletColliderMonoComponent = (bulletUnit.ColliderViewComponent as ColliderViewComponent).ColliderMonoComponent;
            var fishColliderMonoComponent = colliderViewComponent.ColliderMonoComponent;
            if (!bulletColliderMonoComponent.IsCollide(fishColliderMonoComponent))
                return true;

            Vector3 screenPosition;
            if (trackFishUnitId != BulletConfig.DefaultTrackFishUnitId)
            {
                var fishUnitComponent = fishUnit.FishUnitComponent;
                screenPosition = fishUnitComponent.AimPoint.Vector;
            }
            else
                screenPosition = bulletColliderMonoComponent.GetBulletCollidePoint();

            battleLogicComponent.Collide(screenPosition.x, screenPosition.y, bulletUnit.Id, fishUnit.Id);
            return false;
        }
    }

    public static class BattleViewComponentSystem
    {
        /// <summary> 加快获取 CurrentScene 效率, 覆盖重写 Entity 的 CurrentScene() </summary>
        public static Scene CurrentScene(this BattleViewComponent self)
        {
            Scene scene = self.Parent as Scene;
            return BattleConfig.IsAddToCurrentScene ? scene : scene.CurrentScene();
        }

        /// <summary> 加快获取 ZoneScene 效率, 覆盖重写 Entity 的 ZoneScene() </summary>
        public static Scene ZoneScene(this BattleViewComponent self)
        {
            Scene scene = self.Parent as Scene;
            return BattleConfig.IsAddToZoneScene ? scene : scene.Parent.Parent as Scene;
        }

        /// <summary> 拓展实现 Scene 方法, 方便获取 BattleViewComponent </summary>
        public static BattleViewComponent GetBattleViewComponent(this Scene scene)
        {
            Scene battleScene = scene.BattleScene();
            return battleScene.GetComponent<BattleViewComponent>();
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
            if (BattleConfig.IsAddToZoneScene)
                args.ZoneScene.AddComponent<BattleViewComponent>();
        }
    }

    public class AfterCreateCurrentScene_BattleViewComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args)
        {
            if (BattleConfig.IsAddToCurrentScene)
                args.CurrentScene.AddComponent<BattleViewComponent>();
        }
    }
}