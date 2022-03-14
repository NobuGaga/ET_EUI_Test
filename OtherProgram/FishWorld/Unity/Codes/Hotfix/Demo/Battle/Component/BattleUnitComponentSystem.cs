namespace ET.Battle
{
	[ObjectSystem]
	public class BattleUnitComponentAwakeSystem : AwakeSystem<BattleUnitComponent>
	{
		public override void Awake(BattleUnitComponent self)
		{

		}
	}
	
	[ObjectSystem]
	public class BattleUnitComponentDestroySystem : DestroySystem<BattleUnitComponent>
	{
		public override void Destroy(BattleUnitComponent self)
		{

		}
	}
	
	public static class BattleUnitComponentSystem
	{
		//public static void Add(this BattleUnitComponent self, Unit unit)
		//{
		//	Log.Debug("add unit: {0}", unit.Id);
		//	self.TypeUnits.Add((int) unit.UnitType, unit);
		//}

		//public static Unit Get(this BattleUnitComponent self, long id)
		//{
		//	Unit unit = self.GetChild<Unit>(id);
		//	return unit;
		//}

		//public static async ETTask Remove(this BattleUnitComponent self, long id)
		//{
  //          await Game.EventSystem.PublishAsync(new EventType.RemoveUnit() { UnitId = id, CurrentScene = self.DomainScene()});
            
		//	Unit unit = self.GetChild<Unit>(id);
		//	self.TypeUnits.Remove((int) unit.UnitType, unit);
		//	unit?.Dispose();
		//}
		
		//public static HashSet<Unit> GetTypeUnits(this BattleUnitComponent self, UnitType unitType)
		//{
		//	return self.TypeUnits[(int) unitType];
		//}

		//public static Unit GetPlayUnitBySeatId(this BattleUnitComponent self, int seatId)
		//{
		//	HashSet<Unit>  units = self.TypeUnits[(int) UnitType.Player ];
		//	Unit playUnit  = null;
		//	for(int i = 0; i < units.Count; i++)
		//	{
		//		Unit unit = units.ElementAt(i);
		//		int pos = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Pos);
		//		if (pos == seatId)
		//		{
		//			playUnit = unit;
		//			break;
		//		}
		//	}
		//	return playUnit;
		//}
		

	}
}