using ET.EventType;

namespace ET
{
	/// <summary> 进入房间接收协议 </summary>
	[MessageHandler]
	public class M2C_RoomInfoHandler : AMHandler<M2C_RoomInfo>
	{
		protected override void Run(Session session, M2C_RoomInfo message)
		{
			Scene CurrentScene = session.DomainScene().CurrentScene();
			FisheryComponent fisheryComponent = CurrentScene.GetComponent<FisheryComponent>();
			SkillComponent skillComponent = CurrentScene.GetComponent<SkillComponent>();

			fisheryComponent.RoomType = message.RoomId;
			skillComponent.IceEndTime = message.IceTime;
			fisheryComponent.AreaId = message.AreaId;

			ReceiveEnterRoom eventData = new ReceiveEnterRoom() {
				CurrentScene = CurrentScene,
				RoomId = message.RoomId,
				IceEndTime = skillComponent.IceEndTime,
				AreaId = message.AreaId,
			};

			Game.EventSystem.Publish(eventData);
		}
	}
}