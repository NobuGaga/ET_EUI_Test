namespace ET
{
    /// <summary> UnitInfo 工厂方法 </summary>
    public static class UnitInfoFactory
    {
        public static UnitInfo Pop() => MonoPool.Instance.Fetch(typeof(UnitInfo)) as UnitInfo;

        public static UnitInfo PopBulletInfo(int seatId)
        {
            UnitInfo info = Pop();
            info.InitBulletInfo(seatId);
            return info;
        }

        public static UnitInfo PopBulletInfo(int seatId, long trackFishUnitId)
        {
            UnitInfo info = Pop();
            info.InitBulletInfo(seatId, trackFishUnitId);
            return info;
        }

        public static UnitInfo PopBulletInfo(int seatId, long bulletUnitId, long trackFishUnitId)
        {
            UnitInfo info = Pop();
            info.InitBulletInfo(seatId, bulletUnitId, trackFishUnitId);
            return info;
        }

        public static void Push(UnitInfo info)
        {
            info.Dispose();
            MonoPool.Instance.Recycle(info);
        }
    }
}