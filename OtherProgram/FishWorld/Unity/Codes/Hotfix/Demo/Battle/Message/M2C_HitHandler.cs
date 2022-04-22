using ET.EventType;

namespace ET
{
	/// <summary> 击杀鱼接收协议 </summary>
	[MessageHandler]
	public class M2C_HitHandler : AMHandler<M2C_Hit>
	{
		protected override void Run(Session session, M2C_Hit Message)
		{
			Scene zoneScene = session.DomainScene();
			Scene CurrentScene = zoneScene.CurrentScene();

			UnitComponent unitComponent = CurrentScene.GetComponent<UnitComponent>();
			unitComponent.Remove(Message.FishId);

			Game.EventSystem.Publish(new KillFish()
			{
				CurrentScene = CurrentScene,
				Message = Message,
			});
		}
	}
}