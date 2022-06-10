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

        public FishMonoUnit AddFishUnit(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                FishUnitIdList.Add(unitId);

            var fishUnit = Add<FishMonoUnit>(unitId);
            fishUnit.Init();
            return fishUnit;
        }

        public BulletMonoUnit AddBulletUnit(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                BulletUnitIdList.Add(unitId);

            return Add<BulletMonoUnit>(unitId);
        }

        public void ReplaceBulletUnit(long oldId, long newId)
        {
            if (!unitMap.ContainsKey(oldId) || unitMap.ContainsKey(newId))
                return;

            unitMap.Add(newId, unitMap[oldId]);
            unitMap.Remove(oldId);

            for (int index = 0; index < BulletUnitIdList.Count; index++)
            {
                if (BulletUnitIdList[index] == oldId)
                {
                    BulletUnitIdList.RemoveAt(index);
                    break;
                }
            }
            BulletUnitIdList.Add(newId);
        }

        private T Add<T>(long unitId) where T : BattleMonoUnit
        {
            T unit;
            if (unitMap.ContainsKey(unitId))
            {
                Log.Error($"UnitMonoComponent unitMap already add unit, id = { unitId }");
                unit = unitMap[unitId] as T;
            }
            else
            {
                unit = MonoPool.Instance.Fetch(typeof(T)) as T;
                unitMap.Add(unitId, unit);
            }

            unit.UnitId = unitId;
            return unit;
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
            {
                Log.Error($"UnitMonoComponent unitMap not exist unit, id = { unitId }");
                return;
            }
            
            FishUnitIdList.RemoveAt(index);
            PushPool(unitId, unitMap[unitId]);
        }

        /// <summary> 移除残留子弹 </summary>
        public void RemoveUnvalidBulletUnit(long unitId)
        {
            if (!unitMap.ContainsKey(unitId))
                return;

            var bulletMonoUnit = unitMap[unitId] as BulletMonoUnit;
#if !NOT_UNITY

            var transform = bulletMonoUnit.Transform;
            transform.localPosition = new UnityEngine.Vector3(10000, 0, 0);

            if (Define.IsEditor)
                transform.gameObject.name = "bullet_die_node";
#endif
            Remove(unitId);
        }

        public void PushPool(long unitId, BattleMonoUnit unit)
        {
            unit.Dispose();
            unitMap.Remove(unitId);
            MonoPool.Instance.Recycle(unit);
        }
    }
}