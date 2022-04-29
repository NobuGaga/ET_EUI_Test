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
}