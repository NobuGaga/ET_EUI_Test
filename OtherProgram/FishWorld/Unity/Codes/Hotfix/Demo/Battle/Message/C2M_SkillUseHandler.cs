namespace ET
{
	public static class C2M_SkillUseHandler
    {
		public static void C2M_SkillUse(this BattleLogicComponent self, int skillId, long targetId)
		{
            Scene zoneScene = self.ZoneScene();
            SessionComponent sessionComponent = zoneScene.GetComponent<SessionComponent>();
            Session session = sessionComponent.Session;

            C2M_SkillUse message = self.UseSkillInfo;
            message.Set(skillId, targetId);

			try
            {
                session.Send(message);
            }
            catch (System.Exception exception)
            {
                Log.Error(exception);
            }
        }
	}
}