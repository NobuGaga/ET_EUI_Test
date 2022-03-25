
namespace ET
{
    [MessageHandler]
    public class M2C_SkillUseHandler: AMHandler<M2C_SkillUse>
    {
        protected override async ETTask Run(Session session, M2C_SkillUse message)
        {
            if (message.Result != ErrorCode.ERR_Success)
            {
                await ETTask.CompletedTask;
                return;
            }

            Scene currentScene = session.DomainScene().CurrentScene();;
            Unit  myUnit       = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            
            Log.Info($"服务端通知使用技能{message.SkillId}");

            if (myUnit.Id == message.UnitId)
            {
                SkillComponent skillComponent = myUnit.GetComponent<SkillComponent>();
                
                skillComponent.SetSkill(message);
            }

            if (message.SkillId == SkillId.Ice)
            {
                this.IceHandler(session, message);
            }
        }

        public void IceHandler(Session session, M2C_SkillUse message)
        {
        }
    }
}