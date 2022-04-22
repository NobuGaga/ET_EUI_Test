// Battle Review Before Boss Node

// 战斗视图层事件处理

using ET.EventType;

namespace ET
{
    public class BattleEventL2V_ReceiveSkillUseSystem : AEvent<ReceiveSkillUse>
    {
        protected override void Run(ReceiveSkillUse args)
        {
            var battleViewComponent = args.CurrentScene.GetBattleViewComponent();

            switch (args.Message.SkillType)
            {
                case SkillType.Ice:
                    battleViewComponent.FisheryIceSkill();
                    break;
            }
        }
    }

    public class BattleEventL2V_RemoveBulletUnitSystem : AEvent<RemoveBulletUnit>
    {
        protected override void Run(RemoveBulletUnit args) => BattleLogic.Remove(args.UnitId);
    }
}