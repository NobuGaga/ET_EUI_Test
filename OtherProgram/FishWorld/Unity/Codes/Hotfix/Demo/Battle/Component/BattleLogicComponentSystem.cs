// Battle Review Before Boss Node

using ET.EventType;
using System;

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
                objectPool.Recycle(typeof(Unit), new Unit());
                objectPool.Recycle(typeof(NumericComponent), new NumericComponent());
                objectPool.Recycle(typeof(TransformComponent), new TransformComponent());
                objectPool.Recycle(typeof(FishUnitComponent), new FishUnitComponent());
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

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(FishUnitComponent))]
    public class ExecuteTimeLine_BattleLogicComponent : AEventClass<ExecuteTimeLine>
    {
        protected override void Run(object obj)
        {
            var args = obj as ExecuteTimeLine;
            ref long unitId = ref args.UnitId;
            var info = args.Info;
            ref var type = ref info.Type;

            if (type == TimeLineNodeType.ForwardCamera)
            {
                var battleLogicComponent = BattleLogicComponent.Instance;
                Unit unit = battleLogicComponent.UnitComponent.Get(unitId);
                var fishUnitComponent = unit.FishUnitComponent;
                var rotateInfo = fishUnitComponent.RotateInfo;
                rotateInfo.IsFowardMainCamera = !rotateInfo.IsFowardMainCamera;
                return;
            }

            if (type == TimeLineNodeType.Rotate)
            {
                var battleLogicComponent = BattleLogicComponent.Instance;
                Unit unit = battleLogicComponent.UnitComponent.Get(args.UnitId);
                Rotate(unit, info);
                return;
            }

            // 不是状态的都是表现层逻辑
            if (type < TimeLineNodeType.ReadyState)
                return;

            var battleUnit = UnitMonoComponent.Instance.Get(unitId);
            battleUnit.IsCanCollide = info.Type == TimeLineNodeType.ActiveState;
        }

        private void Rotate(Unit self, TimeLineConfigInfo timeLineInfo)
        {
            var fishUnitComponent = self.FishUnitComponent;
            var moveInfo = fishUnitComponent.MoveInfo;
            var rotateInfo = fishUnitComponent.RotateInfo;
            rotateInfo.Reset();

            string[] arguments = timeLineInfo.Arguments;

            if (!float.TryParse(arguments[3], out float time))
                return;

            if (!float.TryParse(arguments[2], out float rotationZ))
                return;

            if (Math.Abs(rotationZ) < 1)
                return;

            // Battle Warning 暂时只当有一条主时间轴, 时间轴总时长跟鱼线时长一致
            long currentTime = TimeHelper.ServerNow();
            long startMoveTime = currentTime - moveInfo.MoveTime;
            long triggerTime = (long)(timeLineInfo.LifeTime * moveInfo.MoveDuration) + startMoveTime;
            uint rotateDuration = (uint)(time * 1000);
            if (currentTime > (triggerTime + rotateDuration))
                return;

            rotateInfo.LocalRotationZ = rotationZ;
            rotateInfo.RotationTime = (int)(currentTime - triggerTime);
            rotateInfo.RotationDuration = rotateDuration;

            var transformComponent = self.TransformComponent;
            var localRotation = transformComponent.Info.LocalRotation;
            var eulerAngles = localRotation.eulerAngles;
            eulerAngles.z += (float)rotateInfo.RotationTime / rotateDuration * rotationZ;
            localRotation.eulerAngles = eulerAngles;
            transformComponent.Info.LocalRotation = localRotation;
        }
    }
}