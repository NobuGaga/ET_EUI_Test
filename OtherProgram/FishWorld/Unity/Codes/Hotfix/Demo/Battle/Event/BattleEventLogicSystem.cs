// Battle Review Before Boss Node

// 战斗逻辑层事件处理

using ET.EventType;

namespace ET
{
    public class BattleEventReceiveSkillUseSystem : AEvent<ReceiveSkillUse>
    {
        protected override void Run(ReceiveSkillUse args)
        {
            switch (args.Message.SkillType)
            {
                case SkillType.Ice:
                    var fisheryComponent = args.CurrentScene.GetComponent<FisheryComponent>();
                    fisheryComponent.FisheryIceSkill(true);
                    break;
            }

            Game.EventSystem.Publish(new FisherySkillStart
            {
                CurrentScene = args.CurrentScene,
                SkillType = args.Message.SkillType,
            });

            Game.EventSystem.Publish(new PlayerSkillStart
            {
                CurrentScene = args.CurrentScene,
                PlayerUnitId = args.Message.UnitId,
                SkillType = args.Message.SkillType,
            });
        }
    }
}