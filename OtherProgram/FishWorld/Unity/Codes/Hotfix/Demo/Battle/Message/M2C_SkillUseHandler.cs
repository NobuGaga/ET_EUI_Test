using ET.EventType;

namespace ET
{
    [MessageHandler]
    [FriendClass(typeof(BattleLogicComponent))]
    public class M2C_SkillUseHandler: AMHandler<M2C_SkillUse>
    {
        protected override void Run(Session session, M2C_SkillUse message)
        {
            if (message.Result != ErrorCode.ERR_Success)
                return;

            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            var skillComponent = battleLogicComponent.SkillComponent;
            skillComponent.UpdateSkill(message);

            switch (message.SkillType)
            {
                case SkillType.Ice:
                    var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
                    fisheryComponent.FisheryIceSkill(true);
                    break;
            }

            var publishData = ReceiveSkillUse.Instance;
            publishData.CurrentScene = currentScene;
            publishData.Message = message;
            Game.EventSystem.PublishClass(publishData);
        }
    }
}