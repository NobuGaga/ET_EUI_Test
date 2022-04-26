using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary> Mono 层 Unit 组件类, 管理鱼 Unit </summary>
    public class UnitMonoComponent
    {
        public static UnitMonoComponent Instance;

        private Dictionary<long, BattleMonoUnit> unitMap = new Dictionary<long, BattleMonoUnit>
                                                                   (ConstHelper.FisheryUnitCount);

        public List<long> FishUnitIdList = new List<long>(ConstHelper.FisheryUnitCount);
        public List<long> BulletUnitIdList = new List<long>(ConstHelper.FisheryUnitCount);

        public void AddFishUnit(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                FishUnitIdList.Add(unitId);
            
            Add<FishMonoUnit>(unitId);
        }

        public void AddBulletUnit(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                BulletUnitIdList.Add(unitId);

            Add<BulletMonoUnit>(unitId);
        }

        public void Add<T>(long unitId) where T : BattleMonoUnit
        {
            if (unitMap.ContainsKey(unitId))
                throw new Exception($"UnitMonoComponent unitMap already add unit, id = { unitId }");

            var unit = MonoPool.Instance.Fetch(typeof(T)) as T;
            unitMap.Add(unitId, unit);
        }

        public T Get<T>(long unitId) where T : BattleMonoUnit
        {
            var unit = Get(unitId);
            if (unit == null)
                return null;

            return unit as T;
        }

        public BattleMonoUnit Get(long unitId)
        {
            if (unitMap.ContainsKey(unitId))
                return unitMap[unitId];

            return null;
        }

        public void Remove(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                return;

            var unit = unitMap[unitId];
            var unitIdList = unit is FishMonoUnit ? FishUnitIdList : BulletUnitIdList;
            for (var index = 0; index < unitIdList.Count; index++)
            {
                if (unitIdList[index] == unitId)
                {
                    unitIdList.RemoveAt(index);
                    break;
                }
            }

            PushPool(unitId, unit);
        }

        public void RemoveFishUnit(int index)
        {
            var unitId = FishUnitIdList[index];
            if (!unitMap.ContainsKey(unitId))
                throw new Exception($"UnitMonoComponent unitMap not exist unit, id = { unitId }");
            
            FishUnitIdList.RemoveAt(index);

            PushPool(unitId, unitMap[unitId]);
        }

        public void PushPool(long unitId, BattleMonoUnit unit)
        {
            unit.Dispose();
            unitMap.Remove(unitId);
            MonoPool.Instance.Recycle(unit);
        }
    }
}