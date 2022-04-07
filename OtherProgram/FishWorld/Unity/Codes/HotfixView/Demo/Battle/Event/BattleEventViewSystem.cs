// 战斗视图层事件处理

using ET.EventType;

namespace ET
{
    public class BattleEventL2V_FisherySkillStartSystem : AEvent<FisherySkillStart>
    {
        protected override void Run(FisherySkillStart args)
        {
            BattleViewComponent battleViewComponent = args.CurrentScene.GetBattleViewComponent();
            switch (args.SkillType)
            {
                case SkillType.Ice:
                    battleViewComponent.FisheryIceSkill();
                    break;
            }
        }
    }

    public class BattleEventL2V_FisherySkillRuningSystem : AEvent<PlayerSkillRuning>
    {
        protected override void Run(PlayerSkillRuning args)
        {
            BattleViewComponent battleViewComponent = args.CurrentScene.GetBattleViewComponent();
            PlayerComponent playerComponent = args.CurrentScene.ZoneScene().GetComponent<PlayerComponent>();

            switch (args.SkillType)
            {
                case SkillType.Ice:
                    break;
                case SkillType.Aim:
                    if (args.PlayerUnitId == playerComponent.MyId)
                        battleViewComponent.SkillShoot();
                    break;
                case SkillType.Laser:

                    break;
            }
        }
    }
}