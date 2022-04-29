using ET.EventType;

namespace ET
{
	/// <summary> 进入房间接收协议 </summary>
	[MessageHandler]
	[FriendClass(typeof(BattleLogicComponent))]
	[FriendClass(typeof(SkillComponent))]
	[FriendClass(typeof(FisheryComponent))]
	public class M2C_RoomInfoHandler : AMHandler<M2C_RoomInfo>
	{
		protected override void Run(Session session, M2C_RoomInfo message)
		{
			var battleLogicComponent= BattleLogicComponent.Instance;
			Scene CurrentScene = battleLogicComponent.CurrentScene;
			FisheryComponent fisheryComponent = CurrentScene.GetComponent<FisheryComponent>();
			var skillComponent = battleLogicComponent.SkillComponent;

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