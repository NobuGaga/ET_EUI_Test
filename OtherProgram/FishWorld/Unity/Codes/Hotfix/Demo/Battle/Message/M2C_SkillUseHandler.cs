namespace ET
{
    [MessageHandler]
    public class M2C_SkillUseHandler: AMHandler<M2C_SkillUse>
    {
        protected override async ETTask Run(Session session, M2C_SkillUse message)
        {
            if (message.Result != ErrorCode.ERR_Success)
            {
                return;
            }

            Scene currentScene = session.DomainScene().CurrentScene();
            Unit  unit         = currentScene.GetComponent<UnitComponent>().Get(message.UnitId);

            if (null == unit)
            {
                Log.Error("释放技能的玩家未找到");
                return;
            }

            Log.Info($"服务端通知使用技能{message.SkillId}");

            SkillComponent skillComponent = unit.GetComponent<SkillComponent>();

            await skillComponent.SetSkill(message);
        }
    }
}