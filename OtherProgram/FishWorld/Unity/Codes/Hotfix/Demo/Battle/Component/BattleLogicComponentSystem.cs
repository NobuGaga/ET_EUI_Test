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
            if (currentScene.Name == "scene_home") return;

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
    [FriendClass(typeof(FishUnitComponent))]
    public class InitTimeLine_BattleLogicComponent : AEventClass<InitTimeLine>
    {
        protected override void Run(object obj)
        {
            var args = obj as InitTimeLine;
            ref long unitId = ref args.UnitId;
            var info = args.Info;
            ref int type = ref info.Type;
            ref float executeTime = ref args.ExecuteTime;

            var battleLogicComponent = BattleLogicComponent.Instance;
            Unit unit = battleLogicComponent.UnitComponent.Get(unitId);
            var fishUnitComponent = unit.FishUnitComponent;

            switch (info.Type)
            {
                case TimeLineNodeType.PauseFishLine:
                    var moveInfo = fishUnitComponent.MoveInfo;
                    moveInfo.IsPause = true;
                    ResumePathMove(unit, executeTime).Coroutine();
                    break;
            }
        }

        private async ETTask ResumePathMove(Unit unit, float executeTime)
        {
            await TimerComponent.Instance.WaitAsync((long)(executeTime * 1000));

            if (unit == null || unit.IsDisposed)
                return;

            var fishUnitComponent = unit.FishUnitComponent;
            var moveInfo = fishUnitComponent.MoveInfo;
            moveInfo.IsPause = true;
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
            ref int type = ref info.Type;

            var battleLogicComponent = BattleLogicComponent.Instance;
            Unit unit = battleLogicComponent.UnitComponent.Get(unitId);

            switch (type)
            {
                case TimeLineNodeType.Rotate:
                    Rotate(unit, info);
                    break;
                case TimeLineNodeType.SpeedChange:
                    SpeedChange(unit, info).Coroutine();
                    break;
                case TimeLineNodeType.ForwardCamera:
                    var fishUnitComponent = unit.FishUnitComponent;
                    var rotateInfo = fishUnitComponent.RotateInfo;
                    rotateInfo.IsFowardMainCamera = !rotateInfo.IsFowardMainCamera;
                    break;
                default:
                    if (type < TimeLineNodeType.ReadyState)
                        break;

                    var battleUnit = UnitMonoComponent.Instance.Get(unitId);
                    battleUnit.IsCanCollide = info.Type == TimeLineNodeType.ActiveState;
                    break;
            }

            // 播放动作在视图层获取 clip 时长
            if (info.IsAutoNext && info.ExecuteTime > 0 &&
                type != TimeLineNodeType.PlayAnimate && type != TimeLineNodeType.PauseFishLine)
                Execute(unitId, info).Coroutine();
        }

        private async ETTask Execute(long unitId, TimeLineConfigInfo info)
        {
            await TimerComponent.Instance.WaitAsync((long)(info.ExecuteTime * FishConfig.MilliSecond));
            var publicData = ExecuteTimeLine.Instance;
            publicData.Set(unitId, info);
            Game.EventSystem.PublishClass(publicData);
        }

        private void Rotate(Unit self, TimeLineConfigInfo timeLineInfo)
        {
            var fishUnitComponent = self.FishUnitComponent;
            var moveInfo = fishUnitComponent.MoveInfo;
            var rotateInfo = fishUnitComponent.RotateInfo;
            rotateInfo.ResetRotateInfo();

            string[] arguments = timeLineInfo.Arguments;

            if (!float.TryParse(arguments[2], out float rotationZ))
                return;

            if (!float.TryParse(arguments[3], out float time))
                return;

            if (Math.Abs(rotationZ) < 1) return;

            // Battle Warning 暂时只当有一条主时间轴, 时间轴总时长跟鱼线时长一致
            long currentTime = TimeHelper.ServerNow();
            long startMoveTime = currentTime - moveInfo.MoveTime;

            // Battle Warning 主时间轴触发时间暂时使用鱼生命周期开始时间(服务端没做, 需要服务器端记录主时间轴触发时间)
            //long triggerTime = (long)(timeLineInfo.LifeTime * moveInfo.MoveDuration) + triggerTimeLineTime;
            long triggerTime = (long)(timeLineInfo.LifeTime * moveInfo.MoveDuration) + startMoveTime;
            uint rotateDuration = (uint)(time * FishConfig.MilliSecond);
            if (currentTime > (triggerTime + rotateDuration))
                return;

            // Battle Warning 欧拉角使用弧度制?
            rotationZ /= 57.29578f;
            rotateInfo.LocalRotationZ = rotationZ;
            rotateInfo.RotationTime = (int)(currentTime - triggerTime);
            rotateInfo.RotationDuration = rotateDuration;

            var transformComponent = self.TransformComponent;
            var localRotation = transformComponent.Info.LocalRotation;
            var eulerAngle = TransformRotateInfoHelper.ToEulerAngle(localRotation);
            eulerAngle.z += (float)rotateInfo.RotationTime / rotateDuration * rotationZ;
            localRotation = TransformRotateInfoHelper.ToQuaternion(eulerAngle);
            transformComponent.Info.LocalRotation = localRotation;
        }

        private async ETTask SpeedChange(Unit self, TimeLineConfigInfo timeLineInfo)
        {
            var fishUnitComponent = self.FishUnitComponent;
            var moveInfo = fishUnitComponent.MoveInfo;
            string[] arguments = timeLineInfo.Arguments;

            if (!int.TryParse(arguments[0], out int changeSpeed))
                return;

            if (!float.TryParse(arguments[1], out float time))
                return;

            // 单位 毫秒
            time *= FishConfig.MilliSecond;
            moveInfo.MoveAcceleration = ((changeSpeed + moveInfo.OriginConfigSpeed - moveInfo.CurrentConfigSpeed)
                                        / moveInfo.OriginConfigSpeed - 1) / time;

            await TimerComponent.Instance.WaitAsync((long)time);

            if (self == null || self.IsDisposed || moveInfo == null) return;

            moveInfo.CurrentConfigSpeed = changeSpeed + moveInfo.OriginConfigSpeed;
            moveInfo.MoveAcceleration = 0;
        }
    }
}