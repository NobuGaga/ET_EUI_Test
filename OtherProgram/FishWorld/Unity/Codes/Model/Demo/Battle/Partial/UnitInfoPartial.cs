using System.Collections.Generic;

namespace ET
{
	public partial class UnitInfo
	{
		public UnitType UnitType => (UnitType)Type;

		public void InitBulletInfo(int seatId) =>
			InitBulletInfo(seatId, BulletConfig.DefaultBulletUnitId, BulletConfig.DefaultTrackFishUnitId);

		public void InitBulletInfo(int seatId, long trackFishUnitId) =>
			InitBulletInfo(seatId, BulletConfig.DefaultBulletUnitId, trackFishUnitId);

		/// <summary> 初始化子弹类型的 Unit Info </summary>
		/// <param name="seatId">座位 ID</param>
		/// <param name="bulletUnitId">子弹 Unit ID</param>
		/// <param name="trackFishUnitId">追踪鱼的 Unit ID</param>
		public void InitBulletInfo(int seatId, long bulletUnitId, long trackFishUnitId)
        {
			UnitId = bulletUnitId;
			ConfigId = BulletConfig.BulletUnitConfigId;
			Type = (int)UnitType.Bullet;

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

			name = null;
        }

		private bool CheckAttributeListValid()
        {
			if (Ks == null || Vs == null || Ks.Count != Vs.Count)
            {
				Log.Error("UnitInfo.GetSeatId() Data List is null or no data");
				return false;
            }

			return true;
        }

		public bool TryGetAttribute(ref long value, int numericType)
        {
			if (!CheckAttributeListValid())
				return false;

			for (ushort index = 0; index < Ks.Count; index++)
            {
				int type = Ks[index];
				if (type == numericType)
                {
					value = Vs[index];
					return true;
                }
            }

			return false;
        }

		public int GetSeatId()
		{
			TryGetSeatId(out long seatId);
			return (int)seatId;
		}

		public bool TryGetSeatId(out long seatId)
		{
			seatId = FisheryConfig.ErrorSeatId;
			return TryGetAttribute(ref seatId, NumericType.Pos);
		}

		public bool TryGetTrackFishUnitId(out long trackFishUnitId)
		{
			trackFishUnitId = BulletConfig.DefaultTrackFishUnitId;
			return TryGetAttribute(ref trackFishUnitId, NumericType.TrackFishId);
		}

		public void Dispose()
        {
			UnitId = 0;
			ConfigId = 0;
			Type = 0;

			if (Ks != null)
				Ks.Clear();

			if (Vs != null)
				Vs.Clear();

			if (SkillList != null)
				SkillList.Clear();

			name = null;
		}
	}
}