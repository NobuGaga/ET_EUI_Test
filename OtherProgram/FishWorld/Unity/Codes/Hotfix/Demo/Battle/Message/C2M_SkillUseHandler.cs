namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
	public static class C2M_SkillUseHandler
    {
		public static void C2M_SkillUse(this BattleLogicComponent self, int skillType, long trackFishUnitId)
		{
            var sessionComponent = self.SessionComponent;
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