using ET.EventType;

using BattleMonoComponentExtension = ET.BattleMonoComponentSystem;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleViewComponent))]
    public class BattleViewComponentAwakeSystem : AwakeSystem<BattleViewComponent>
    {
        public override void Awake(BattleViewComponent self)
        {
            BattleMonoComponent.Instance.EnterBattle(BattleMonoComponentExtension.Collide,
                                                     BattleMonoComponentExtension.RemoveFishUnit);

            BattleViewComponent.Instance = self;

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
        public override void Destroy(BattleViewComponent self)
        {
            BattleMonoComponent.Instance.ExitBattle();
            BattleViewComponent.Instance = null;
            self.InstantiateFishStack.Clear();
        }
    }

    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BulletLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public class BattleViewComponentUpdateSystem : UpdateSystem<BattleViewComponent>
    {
        public override void Update(BattleViewComponent self)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            if (currentScene == null)
                return;

            self.CurrentInstantiateCount = 0;
            var unitComponent = currentScene.GetComponent<UnitComponent>();
            while (self.CurrentInstantiateCount < BattleConfig.FrameInstantiateObjectCount &&
                   self.InstantiateFishStack.Count > 0)
            {
                long fishUnitId = self.InstantiateFishStack.Pop();
                Unit fishUnit = unitComponent.Get(fishUnitId);
                if (fishUnit == null || fishUnit.IsDisposed)
                    continue;

                fishUnit.InstantiateGameObject();
                self.CurrentInstantiateCount++;
            }

            // 有序更新, 统一在一个 Update 里进行
            currentScene.GetComponent<SkillComponent>().FixedUpdateBeforeFish();
            currentScene.GetComponent<SkillComponent>().UpdateBeforeBullet();
        }
    }

    public class AfterCreateCurrentScene_BattleViewComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args) =>
                                args.CurrentScene.AddComponent<BattleViewComponent>();
    }
}