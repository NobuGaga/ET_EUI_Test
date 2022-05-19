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
            var fishUnit = unitComponent.GetChild<Unit>(fishUnitId);
            var fishUnitComponent = fishUnit.FishUnitComponent;
            fishUnitComponent.MoveInfo.IsPause = true;
            var battleMonoUnit = UnitMonoComponent.Instance.Get(fishUnitId);
            battleMonoUnit.IsCanCollide = false;
            fishUnit.PlayAnimation(MotionType.Die, false);
            RemoveUnit(unitComponent, fishUnitId).Coroutine();
        }

        private async ETTask RemoveUnit(UnitComponent unitComponent, long fishUnitId)
        {
            await TimerComponent.Instance.WaitAsync(417);
            unitComponent.Remove(fishUnitId);
        }
    }
}