// Battle Review Before Boss Node

// 战斗视图层事件处理

using ET.EventType;

namespace ET
{
    public class BattleEventL2V_ReceiveSkillUseSystem : AEventClass<ReceiveSkillUse>
    {
        protected override void Run(object args)
        {
            switch ((args as ReceiveSkillUse).Message.SkillType)
            {
                case SkillType.Ice:
                    FisheryViewComponentSystem.FisheryIceSkill();
                    break;
            }
        }
    }

    public class BattleEventL2V_RemoveBulletUnitSystem : AEventClass<RemoveBulletUnit>
    {
        protected override void Run(object args) => TransformMonoHelper.Remove((args as RemoveBulletUnit).UnitId);
    }

    // Battle TODO 临时处理击杀鱼表现
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(AnimatorComponent))]
    public class BattleEventL2V_KillFishSystem : AEventClass<KillFish>
    {
        protected override void Run(object obj)
        {
            var args = obj as KillFish;
            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
            long fishUnitId = args.Message.FishId;
            var battleMonoUnit = UnitMonoComponent.Instance.Get(fishUnitId);
            if (battleMonoUnit != null)
                battleMonoUnit.IsCanCollide = false;

            var fishUnit = unitComponent.GetChild<Unit>(fishUnitId);
            if (fishUnit == null || fishUnit.IsDisposed)
                return;

            var fishUnitComponent = fishUnit.FishUnitComponent;
            fishUnitComponent.MoveInfo.IsPause = true;

            var clip = fishUnit.PlayAnimation(MotionTypeHelper.Get(MotionType.Die), 0, false);
            RemoveUnit(unitComponent, fishUnitId, clip != null ? clip.length : 0.5f).Coroutine();
        }

        private async ETTask RemoveUnit(UnitComponent unitComponent, long fishUnitId, float time)
        {
            await TimerComponent.Instance.WaitAsync((long)(time * FishConfig.MilliSecond));
            unitComponent.Remove(fishUnitId);
        }
    }
}