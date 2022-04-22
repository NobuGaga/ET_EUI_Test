using ET.EventType;

namespace ET
{
	/// <summary> 击杀鱼接收协议 </summary>
	[MessageHandler]
	[FriendClass(typeof(BattleLogicComponent))]
	public class M2C_HitHandler : AMHandler<M2C_Hit>
	{
		protected override void Run(Session session, M2C_Hit Message)
		{
			Scene zoneScene = session.DomainScene();
			Scene CurrentScene = zoneScene.CurrentScene();

			long fishUnitId = Message.FishId;
			var fishUnitIdList = BattleLogicComponent.Instance.FishUnitIdList;
			for (int index = 0; index < fishUnitIdList.Count; index++)
				if (fishUnitIdList[index] == fishUnitId)
                {
					fishUnitIdList.RemoveAt(index);
					break;
                }

			UnitComponent unitComponent = CurrentScene.GetComponent<UnitComponent>();
			unitComponent.Remove(fishUnitId);

			Game.EventSystem.Publish(new KillFish()
			{
				CurrentScene = CurrentScene,
				Message = Message,
			});
		}
	}
}