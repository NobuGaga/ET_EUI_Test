// Battle Review Before Boss Node

using ET.EventType;

namespace ET
{
	/// <summary> 发射子弹接收协议 </summary>
	[MessageHandler]
	[FriendClass(typeof(BattleLogicComponent))]
	public class M2C_FireHandler : AMHandler<M2C_Fire>
	{
		protected override void Run(Session session, M2C_Fire message)
		{
			var battleLogicComponent = BattleLogicComponent.Instance;
			Scene CurrentScene = battleLogicComponent.CurrentScene;
			var bulletLogicComponent = battleLogicComponent.BulletLogicComponent;

			var fisheryComponent = battleLogicComponent.FisheryComponent;
			int seatId = fisheryComponent.GetSeatId(message.UnitId);
			UnitInfo UnitInfo = bulletLogicComponent.PopUnitInfo(seatId, message.BulletId, message.TrackFishUnitId);

			var publishData = ReceiveFire.Instance;
			publishData.Set(CurrentScene, UnitInfo, message);
			Game.EventSystem.PublishClass(publishData);
		}
	}
}