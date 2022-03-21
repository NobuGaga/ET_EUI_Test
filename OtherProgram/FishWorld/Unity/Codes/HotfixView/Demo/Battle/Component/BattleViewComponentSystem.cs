using System.Linq;
using ET.EventType;
using System.Collections.Generic;

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

    #endregion

    #region Life Circle

    [ObjectSystem]
    public sealed class BattleViewComponentAwakeSystem : AwakeSystem<BattleViewComponent>
    {
        public override void Awake(BattleViewComponent self)
        {
            // Mono 层方法只能在 View 层调用
            BattleLogic.Init();
            // 战斗用另外一个对象池组件, 生命周期跟战斗视图组件
            // 只用来管理鱼跟子弹
            self.AddComponent<ObjectPoolComponent>();
        }
    }

    [ObjectSystem]
    public sealed class BattleViewComponentUpdateSystem : UpdateSystem<BattleViewComponent>
    {
        public override void Update(BattleViewComponent self) => self.Update();
    }

    [ObjectSystem]
    public sealed class BattleViewComponentDestroySystem : DestroySystem<BattleViewComponent>
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
        public static void Update(this BattleViewComponent self)
        {
            // 需要保证 BattleLogicComponent 跟 BattleViewComponent 挂在同一个 Scene 上
            // 都通过 BattleTestConfig 的标记进行判断
            BattleLogicComponent battleLogicComponent = self.Parent.GetComponent<BattleLogicComponent>();
            UnitComponent unitComponent = battleLogicComponent.GetUnitComponent();
            if (unitComponent == null)
                return;
            
            HashSet<Unit> fishList = unitComponent.GetFishList();
            for (int index = 0; index < fishList.Count; index++)
            {
                Unit unit = fishList.ElementAt(index);
                unit.BattleUnitViewComponent().Update();
                // 原逻辑没有做渲染鱼数据分离, 先调用了渲染再调用逻辑更新
                unit.BattleUnitLogicComponent().FixedUpdate();
            }
        }
    }

    #endregion
}