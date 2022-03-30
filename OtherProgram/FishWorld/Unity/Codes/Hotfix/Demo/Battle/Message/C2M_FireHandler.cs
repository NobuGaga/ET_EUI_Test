namespace ET
{
	public static class C2M_FireHandler
	{
		public static void C2M_Fire(this BattleLogicComponent self, long bulletUnitId,
                                            float touchPosX, float touchPosY, int cannonStack,
                                            long trackFishUnitId)
		{
            Scene zoneScene = self.ZoneScene();
            SessionComponent sessionComponent = zoneScene.GetComponent<SessionComponent>();
            Session session = sessionComponent.Session;
            
            C2M_Fire message = self.FireInfo;
            message.Set(bulletUnitId, touchPosX, touchPosY, cannonStack, trackFishUnitId);

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