using ET.EventType;

namespace ET
{
    [MessageHandler]
    public class M2C_SkillUseHandler: AMHandler<M2C_SkillUse>
    {
        protected override void Run(Session session, M2C_SkillUse message)
        {
            if (message.Result != ErrorCode.ERR_Success)
                return;

            Scene zoneScene = session.DomainScene();
            Scene currentScene = zoneScene.CurrentScene();
            SkillComponent skillComponent = currentScene.GetComponent<SkillComponent>();
            skillComponent.UpdateSkill(message);

            Game.EventSystem.Publish(new ReceiveSkillUse()
            {
                CurrentScene = currentScene,
                Message = message,
            });
        }
    }
}