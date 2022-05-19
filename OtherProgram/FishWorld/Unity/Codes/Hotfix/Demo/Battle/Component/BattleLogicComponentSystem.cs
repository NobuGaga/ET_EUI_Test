// Battle Review Before Boss Node

using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public class BattleLogicComponentAwakeSystem : AwakeSystem<BattleLogicComponent>
    {
        public override void Awake(BattleLogicComponent self)
        {
            BattleLogicComponent.Instance = self;

            self.FireInfo = new C2M_Fire();
            self.HitInfo = new C2M_Hit();
            self.UseSkillInfo = new C2M_SkillUse();
        }
    }

    [ObjectSystem]
    public class BattleLogicComponentDestroySystem : DestroySystem<BattleLogicComponent>
    {
        public override void Destroy(BattleLogicComponent self)
        {
            BattleLogicComponent.Instance = null;

            self.CurrentScene = null;
            self.ZoneScene = null;

            self.SessionComponent = null;

            self.UnitComponent = null;
            self.BulletLogicComponent = null;
            self.SkillComponent = null;
            self.FisheryComponent = null;

            self.FireInfo = null;
            self.HitInfo = null;
            self.UseSkillInfo = null;

            self.Result_Unit = null;
            self.Action_Unit_Bool = null;
            self.BreakFunc_Unit_Integer = null;
        }
    }

    public class AfterCreateZoneScene_BattleLogicComponent : AEvent<AfterCreateZoneScene>
    {
        protected override void Run(AfterCreateZoneScene args)
        {
            var objectPool = ObjectPool.Instance;
            for (int index = 0; index < ConstHelper.PreCreateFishClassCount; index++)
            {
                objectPool.Recycle(new Unit());
                objectPool.Recycle(new NumericComponent());
                objectPool.Recycle(new TransformComponent());
                objectPool.Recycle(new FishUnitComponent());
            }
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    public class AfterCreateCurrentScene_BattleLogicComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args)
        {
            Scene currentScene = args.CurrentScene;

            // Battle TODO 战斗场景才添加战斗逻辑组件
            if (currentScene.Name == "scene_home")
                return;

            var self = currentScene.AddComponent<BattleLogicComponent>();

            self.CurrentScene = currentScene;
            self.ZoneScene = currentScene.Parent.Parent as Scene;

            self.SessionComponent = self.ZoneScene.GetComponent<SessionComponent>();

            self.UnitComponent = currentScene.GetComponent<UnitComponent>();
            self.BulletLogicComponent = currentScene.AddComponent<BulletLogicComponent>();
            self.SkillComponent = currentScene.AddComponent<SkillComponent>();
            self.FisheryComponent = currentScene.AddComponent<FisheryComponent>();
        }
    }

    public class ExecuteTimeLine_BattleLogicComponent : AEventClass<ExecuteTimeLine>
    {
        protected override void Run(object obj)
        {
            var args = obj as ExecuteTimeLine;
            var info = args.Info;

            // 不是状态的都是表现层逻辑
            if (info.Type < TimeLineNodeType.ReadyState)
                return;

            var battleUnit = UnitMonoComponent.Instance.Get(args.UnitId);
            battleUnit.IsCanCollide = info.Type == TimeLineNodeType.ActiveState;
        }
    }
}