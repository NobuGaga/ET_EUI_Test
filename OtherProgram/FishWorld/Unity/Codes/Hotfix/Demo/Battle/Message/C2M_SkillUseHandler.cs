namespace ET
{
	public static class C2M_SkillUseHandler
    {
		public static void C2M_SkillUse(this BattleLogicComponent self, int skillType, long trackFishUnitId)
		{
            Scene zoneScene = self.ZoneScene();
            SessionComponent sessionComponent = zoneScene.GetComponent<SessionComponent>();
            Session session = sessionComponent.Session;

            C2M_SkillUse message = self.UseSkillInfo;
            message.Set(skillType, trackFishUnitId);

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