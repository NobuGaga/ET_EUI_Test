using System.Collections.Generic;

namespace ET
{
	public partial class C2M_Fire
    {
		public void Set(long bulletUnitId, float touchPosX, float touchPosY, int cannonStack,
						long trackFishUnitId)
        {
			BulletId = bulletUnitId;

			PosX = (int)(touchPosX * BulletConfig.TouchScreenPosFix);
			PosY = (int)(touchPosY * BulletConfig.TouchScreenPosFix);

			Stack = cannonStack;
			Step = 10;
			Time = TimeHelper.ServerNow();

			if (FishId == null)
				FishId = new List<long>();

			FishId.Clear();

			if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId)
				FishId.Add(trackFishUnitId);
		}
	}

	public partial class M2C_Fire
    {
		public float TouchPosX => PosX / BulletConfig.TouchScreenPosFix;

		public float TouchPosY => PosY / BulletConfig.TouchScreenPosFix;

		public long TrackFishUnitId
		{
			get
			{
				if (FishId != null && FishId.Count > 0)
					return FishId[0];

				return ConstHelper.DefaultTrackFishUnitId;
			}
		}
	}
}