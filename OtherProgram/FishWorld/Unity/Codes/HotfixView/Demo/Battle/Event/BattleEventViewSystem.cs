// Battle Review Before Boss Node

// 战斗视图层事件处理

using ET.EventType;

namespace ET
{
    public class BattleEventL2V_ReceiveSkillUseSystem : AEventClass<ReceiveSkillUse>
    {
        protected override void Run(object args)
        {
            var battleViewComponent = BattleViewComponent.Instance;

            switch ((args as ReceiveSkillUse).Message.SkillType)
            {
                case SkillType.Ice:
                    battleViewComponent.FisheryIceSkill();
                    break;
            }
        }
    }

    public class BattleEventL2V_RemoveBulletUnitSystem : AEventClass<RemoveBulletUnit>
    {
        protected override void Run(object args) => TransformMonoHelper.Remove((args as RemoveBulletUnit).UnitId);
    }
}