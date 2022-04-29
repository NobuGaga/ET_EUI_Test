namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
	public static class C2M_HitHandler
    {
		public static void C2M_Hit(this BattleLogicComponent self, float screenPosX, float screenPosY,
                                                                   long bulletUnitId, long fishUnitId)
		{
            var sessionComponent = self.SessionComponent;
            Session session = sessionComponent.Session;

            C2M_Hit message = self.HitInfo;
            message.Set(screenPosX, screenPosY, bulletUnitId, fishUnitId);

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