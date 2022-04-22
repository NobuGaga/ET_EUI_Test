// Battle Review Before Boss Node

namespace ET
{
	/// <summary> 区域转换接收协议 </summary>
	[MessageHandler]
	[FriendClass(typeof(FisheryComponent))]
	public class M2C_ExchangeAreaHandler : AMHandler<M2C_ExchangeArea>
	{
		protected override void Run(Session session, M2C_ExchangeArea message)
		{
			Scene currentScene = session.DomainScene().CurrentScene();
			FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
			fisheryComponent.AreaId = message.AreaId;
			fisheryComponent.QuickMoveAllFish();

			Game.EventSystem.Publish(new EventType.ReceiveExchangeArea()
			{
				FisheryComponent = fisheryComponent,
			});
		}
	}
}