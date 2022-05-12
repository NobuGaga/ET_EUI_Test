using System.Collections.Generic;

namespace ET
{
	public partial class UnitInfo
	{
		/// <summary> 初始化子弹类型的 Unit Info </summary>
		/// <param name="seatId">座位 ID</param>
		/// <param name="bulletUnitId">子弹 Unit ID</param>
		/// <param name="trackFishUnitId">追踪鱼的 Unit ID</param>
		public void InitBulletInfo(int seatId, long bulletUnitId, long trackFishUnitId)
        {
			UnitId = bulletUnitId;
			ConfigId = BulletConfig.BulletUnitConfigId;
			Type = UnitType.Bullet;

			if (Ks == null)
				Ks = new List<int>() { NumericType.Pos, NumericType.TrackFishId };
			else
            {
				Ks.Add(NumericType.Pos);
				Ks.Add(NumericType.TrackFishId);
			}

			if (Vs == null)
				Vs = new List<long>() { seatId, trackFishUnitId };
			else
			{
				Vs.Add(seatId);
				Vs.Add(trackFishUnitId);
			}
        }

		public bool TryGetAttribute(ref long value, int numericType)
        {
			if (Ks == null || Vs == null || Ks.Count != Vs.Count)
			{
				Log.Error("UnitInfo.TryGetAttribute() Data List is null or no data");
				return false;
			}

			for (ushort index = 0; index < Ks.Count; index++)
            {
				if (Ks[index] == numericType)
                {
					value = Vs[index];
					return true;
                }
            }

			return false;
        }

		public int GetSeatId()
		{
			long seatId = FisheryConfig.ErrorSeatId;
			TryGetAttribute(ref seatId, NumericType.Pos);
			return (int)seatId;
		}

		public bool TryGetTrackFishUnitId(out long trackFishUnitId)
		{
			trackFishUnitId = ConstHelper.DefaultTrackFishUnitId;
			return TryGetAttribute(ref trackFishUnitId, NumericType.TrackFishId);
		}

		public void Dispose()
        {
			UnitId = 0;
			ConfigId = 0;
			Type = 0;

			Ks?.Clear();
			Vs?.Clear();
		}
	}
}