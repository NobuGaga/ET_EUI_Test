using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    #region Event

    // 先按 ZoneScene 的生命周期走, 后面看设计合理性是挂在 Current 还是 Zone
    // Event 的执行不依赖顺序
    // 目前订阅了 AfterCreateZoneScene 事件的处理只有 AfterCreateZoneScene_AddComponent
    // 避免别的事件执行顺序的依赖, 战斗相关组件释放要做好解耦断引用
    public class AfterCreateZoneScene_BattleLogicComponent : AEvent<AfterCreateZoneScene>
    {
        protected override async ETTask Run(AfterCreateZoneScene args)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                args.ZoneScene.AddComponent<BattleLogicComponent>();

            await ETTask.CompletedTask;
        }
    }

    public class AfterCreateCurrentScene_BattleLogicComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(AfterCreateCurrentScene args)
        {
            if (BattleTestConfig.IsAddBattleToCurrent)
                args.CurrentScene.AddComponent<BattleLogicComponent>();
            
            await ETTask.CompletedTask;
        }
    }

    #endregion

    #region Life Circle

    [ObjectSystem]
    public sealed class BattleLogicComponentAwakeSystem : AwakeSystem<BattleLogicComponent>
    {
        public override void Awake(BattleLogicComponent self)
        {
            // Battle TODO
        }
    }

    /// <summary>
    /// Battle TODO
    /// 先在 Update 里实现数据逻辑的更新, 后面做插值后再改成 FixedUpdate 进行数据逻辑的更新
    /// </summary>
    //[ObjectSystem]
    //public sealed class BattleLogicComponentUpdateSystem : UpdateSystem<BattleLogicComponent>
    //{
    //    public override void Update(BattleLogicComponent self)
    //    {
    //        // Battle TODO
    //    }
    //}

    [ObjectSystem]
    public sealed class BattleLogicComponentDestroySystem : DestroySystem<BattleLogicComponent>
    {
        public override void Destroy(BattleLogicComponent self)
        {
            // Battle TODO
        }
    }

    #endregion

    #region Base Function

    public static class BattleLogicComponentSystem
    {
        public static void FixedUpdate(this BattleLogicComponent self)
        {
            UnitComponent unitComponent = self.GetUnitComponent();
            if (unitComponent == null)
                return;

            HashSet<Unit> fishList = unitComponent.GetFishList();
            foreach (Unit fishUnit in fishList)
                fishUnit.FixedUpdate();
        }

        /// <summary>
        /// 方便以后将 UnitComponent 挂到别的 Scene, 在 BattleLogicComponent 定义接口获取
        /// </summary>
        public static UnitComponent GetUnitComponent(this BattleLogicComponent self)
        {
            Scene currentScene;
            if (BattleTestConfig.IsAddBattleToZone)
                // CurrentScene 全局只有一个
                currentScene = self.ZoneScene().CurrentScene();
            else
                currentScene = self.DomainScene();

            // 登录界面还未创建 CurrentScene 需要作空判断, 如果 BattleLogicComponent 挂 ZoneScene 的话
            if (currentScene == null)
                return null;

            // Battle Warning 目前使用统一挂在 CurrentScene 的 UnitComponent
            return currentScene.GetComponent<UnitComponent>();
        }
    }

    #endregion
}