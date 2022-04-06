namespace ET
{
	public partial class C2M_Hit
    {
		public void Set(float screenPosX, float screenPosY, long bulletUnitId, long fishUnitId)
        {
			PosX = (int)(screenPosX * BulletConfig.TouchScreenPosFix);
			PosY = (int)(screenPosY * BulletConfig.TouchScreenPosFix);

			BulletId = bulletUnitId;
			FishId = fishUnitId;
		}
	}

	public partial class M2C_Hit
	{
		public float ScreenPosX => PosX / BulletConfig.TouchScreenPosFix;

		public float ScreenPosY => PosY / BulletConfig.TouchScreenPosFix;
	}
}