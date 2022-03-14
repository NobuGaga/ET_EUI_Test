using ET.EventType;

namespace ET.Battle
{
    // 先按 ZoneScene 的生命周期走, 后面看设计合理性是挂在 Current 还是 Zone
    // Event 的执行不依赖顺序
    // 目前订阅了 AfterCreateZoneScene 事件的处理只有 AfterCreateZoneScene_AddComponent
    // 避免别的事件执行顺序的依赖, 战斗相关组件释放要做好解耦断引用
    public class AfterCreateZoneScene_BattleLogicInit : AEvent<AfterCreateZoneScene>
    {
        protected override async ETTask Run(AfterCreateZoneScene args)
        {
            if (BattleTestConfig.IsAddBattleToZone)
                args.ZoneScene.AddComponent<BattleLogicComponent>();
            await ETTask.CompletedTask;
        }
    }

    public class AfterCreateCurrentScene_BattleLogicInit : AEvent<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(AfterCreateCurrentScene args)
        {
            if (BattleTestConfig.IsAddBattleToCurrent)
                args.CurrentScene.AddComponent<BattleLogicComponent>();
            await ETTask.CompletedTask;
        }
    }

    public sealed class BattleLogicComponentAwakeSystem : AwakeSystem<BattleLogicComponent>
    {
        public override void Awake(BattleLogicComponent self)
        {
            self.AddComponent<BattleUnitComponent>();
        }
    }

    public static class BattleLogicComponentSystem
    {
        public static BattleUnit CreateBattleUnit(this BattleLogicComponent self, UnitInfo unitInfo)
        {
            BattleUnitComponent battleUnitCom = self.GetComponent<BattleUnitComponent>();

            // isFromPool 参数为加入对象池, 对象池是单层结构, Entity 加入池中时只会加自身加入池中
            // 不会把其下的子组件或者子孩子加入池中
            // 暂时使用标记来设置是否使用对象池
            bool isFromPool = BattleTestConfig.IsUseModelPool;
            return battleUnitCom.AddChildWithId<BattleUnit, UnitInfo>(unitInfo.UnitId, unitInfo, isFromPool);
        }
    }
}