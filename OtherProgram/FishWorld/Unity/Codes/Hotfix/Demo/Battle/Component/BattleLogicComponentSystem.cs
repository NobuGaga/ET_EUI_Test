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
            Scene currentScene = self.Parent as Scene;
            if (currentScene.SceneType != SceneType.Current)
                currentScene = currentScene.CurrentScene();

            // Bullet Unit Component 要挂在 Current Scene 上面, 释放的时候跟 Unit Component 保持一直
            currentScene.AddComponent<BulletLogicComponent>();
        }
    }

    /// <summary>
    /// Battle TODO
    /// 先在 Update 里实现数据逻辑的更新, 后面做插值后再改成 FixedUpdate 进行数据逻辑的更新
    /// </summary>
    //[ObjectSystem]
    //public sealed class BattleLogicComponentFixedUpdateSystem : FixedUpdateSystem<BattleLogicComponent>
    //{
    //    public override void FixedUpdate(BattleLogicComponent self)
    //    {
    //        UnitComponent unitComponent = self.GetUnitComponent();
    //        if (unitComponent == null)
    //            return;

    //        foreach (Unit fishUnit in unitComponent.GetFishUnitList())
    //            fishUnit.FixedUpdate();
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
        /// <summary> 加快获取 CurrentScene 效率 </summary>
        public static Scene CurrentScene(this BattleLogicComponent self)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                // CurrentScene 全局只有一个
                return self.ZoneScene().CurrentScene();
            
            return self.DomainScene();
        }

        /// <summary>
        /// 获取战斗组件所在的 Scene, 所有战斗相关的管理组件都在这个 Scene 下
        /// 通过在 BattleLogicComponent 的 Awake 跟 BattleViewComponent 的 Awake 中
        /// 将战斗用的组件添加到 Scene 上
        /// 因为 Game.Scene 里获取到的可能为空(ZoneScene or CurrentScene)
        /// 所以调用的时候随便传入一个 Scene (客户端只有一个 ZoneScene 跟一个 CurrentScene)
        /// </summary>
        public static Scene BattleScene(this Scene self)
        {
            bool isZone = self.SceneType == SceneType.Zone;
            if ((BattleTestConfig.IsAddBattleToZone && !isZone) ||
                (BattleTestConfig.IsAddBattleToCurrent && isZone))
                return self.CurrentScene();

            return self;
        }

        /// <summary> 拓展实现 Scene 方法, 方便获取 BattleLogicComponent </summary>
        public static BattleLogicComponent GetBattleLogicComponent(this Scene scene)
        {
            Scene battleScene = scene.BattleScene();
            return battleScene.GetComponent<BattleLogicComponent>();
        }

        public static long GetSelfUnitId(this BattleLogicComponent self)
        {
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.CurrentScene());
            return selfPlayerUnit.Id;
        }

        /// <summary>
        /// 方便以后将 UnitComponent 挂到别的 Scene, 在 BattleLogicComponent 定义接口获取
        /// </summary>
        public static UnitComponent GetUnitComponent(this BattleLogicComponent self)
        {
            Scene currentScene = self.CurrentScene();

            // 登录界面还未创建 CurrentScene 需要作空判断, 如果 BattleLogicComponent 挂 ZoneScene 的话
            if (currentScene == null)
                return null;

            // Battle Warning 目前使用统一挂在 CurrentScene 的 UnitComponent
            return currentScene.GetComponent<UnitComponent>();
        }
    }

    #endregion
}