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
			var battleLogicComponent = BattleLogicComponent.Instance;
			Scene CurrentScene = battleLogicComponent.CurrentScene;

			var publishData = KillFish.Instance;
			publishData.CurrentScene = CurrentScene;
			publishData.Message = Message;
			Game.EventSystem.PublishClass(publishData);
		}
	}
}