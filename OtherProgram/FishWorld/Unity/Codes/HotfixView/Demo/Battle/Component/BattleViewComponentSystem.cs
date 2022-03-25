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

    public class AfterShoot_BattleViewComponent : AEvent<AfterShoot>
    {
        protected override async ETTask Run(AfterShoot args)
        {
            BattleViewComponent battleViewComponent = args.CurrentScene.GetBattleViewComponent();
            //battleViewComponent.CallLogicShootBullet()

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
            //BattleLogicComponent battleLogicComponent = self.DomainScene().GetBattleLogicComponent();
            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            if (unitComponent == null)
                return;

            HashSet<Unit> fishList = unitComponent.GetFishUnitList();
            foreach (Unit unit in fishList)
            {
                BattleUnitLogicComponent battleUnitLogicComponent = unit.GetComponent<BattleUnitLogicComponent>();
                if (!battleUnitLogicComponent.IsUpdate)
                    continue;

                unit.FixedUpdate();
                unit.Update();
            }
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
        private static Scene CurrentScene(this BattleViewComponent self) => self.DomainScene().CurrentScene();

        /// <summary> 拓展实现 Scene 方法, 方便获取 BattleViewComponent </summary>
        public static BattleViewComponent GetBattleViewComponent(this Scene scene)
        {
            Scene battleScene = scene.BattleScene();
            return battleScene.GetComponent<BattleViewComponent>();
        }

        public static void RotateCannon(this BattleViewComponent self, ref float shootDirX, ref float shootDirY)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            self.RotateCannon(selfSeatId, ref shootDirX, ref shootDirY);
        }

        public static void RotateCannon(this BattleViewComponent self, int seatId, ref float shootDirX, ref float shootDirY)
        {
            CannonHelper.CalcCannonRotation(self.CurrentScene(), seatId, shootDirX, shootDirY);
        }

        public static void CallLogicShootBullet(this BattleViewComponent self, ref float shootDirX, ref float shootDirY)
        {
            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int selfSeatId = fisheryComponent.GetSelfSeatId();
            self.CallLogicShootBullet(selfSeatId, ref shootDirX, ref shootDirY);
        }

        /// <summary> 通知战斗逻辑发射子弹, 这里做一些视图层的处理然后再把数据传到逻辑层 </summary>
        public static void CallLogicShootBullet(this BattleViewComponent self, int seatId, ref float shootDirX, ref float shootDirY)
        {
            self.RotateCannon(seatId, ref shootDirX, ref shootDirY);

            Scene currentScene = self.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();

            CannonComponent cannonComponent = fisheryComponent.GetSelfCannonComponent();
            cannonComponent.TryGetLocalRotation(out Quaternion localRotation);

            // 本来想将引用挂在 CannonComponent 上, 奈何 CannonComponent 定义在 Model 层
            GlobalComponent globalComponent = GlobalComponent.Instance;
            Transform shootPointTrans = globalComponent.CannonSeatLayout.GetShootPoint(seatId);
            Vector3 ShootScreenPosition = globalComponent.CannonCamera.WorldToScreenPoint(shootPointTrans.position);

            // 需要在视图层初始化炮台信息后传到逻辑层, 再由逻辑层使用数据
            CannonShootInfo info = CannonShootHelper.PopInfo();
            info.Init(localRotation, ShootScreenPosition);

            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            battleLogicComponent.ShootBullet(info);
        }
    }

    #endregion
}