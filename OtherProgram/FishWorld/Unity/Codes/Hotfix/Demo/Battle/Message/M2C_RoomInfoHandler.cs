using ET.EventType;

namespace ET
{
	/// <summary> 进入房间接收协议 </summary>
	[MessageHandler]
	public class M2C_RoomInfoHandler : AMHandler<M2C_RoomInfo>
	{
		protected override async ETTask Run(Session session, M2C_RoomInfo message)
		{
			Scene CurrentScene = session.DomainScene().CurrentScene();
			FisheryComponent FisheryComponent = CurrentScene.GetComponent<FisheryComponent>();

			FisheryComponent.RoomType = message.RoomId;
			FisheryComponent.LeftIceTime = message.LeftIceTime;
			FisheryComponent.AreaId = message.AreaId;

			ReceiveEnterRoom eventData = new ReceiveEnterRoom() {
				CurrentScene = CurrentScene,
				FisheryComponent = FisheryComponent,
			};

			Game.EventSystem.Publish(eventData);

			await ETTask.CompletedTask;
		}
	}
}