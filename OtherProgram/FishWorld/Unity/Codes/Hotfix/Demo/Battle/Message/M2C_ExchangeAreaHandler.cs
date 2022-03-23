using ET.EventType;

namespace ET
{
	/// <summary> 区域转换接收协议 </summary>
	[MessageHandler]
	public class M2C_ExchangeAreaHandler : AMHandler<M2C_ExchangeArea>
	{
		protected override async ETTask Run(Session session, M2C_ExchangeArea message)
		{
			Scene CurrentScene = session.DomainScene().CurrentScene();
			FisheryComponent FisheryComponent = CurrentScene.GetComponent<FisheryComponent>();

			FisheryComponent.AreaId = message.AreaId;

			AfterExchangeArea eventData = new AfterExchangeArea() {
				FisheryComponent = FisheryComponent,
			};

			Game.EventSystem.Publish(eventData);

			await ETTask.CompletedTask;
		}
	}
}