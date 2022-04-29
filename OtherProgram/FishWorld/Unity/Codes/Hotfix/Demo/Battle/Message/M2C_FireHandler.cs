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

			long unitId = message.UnitId;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(CurrentScene);
			// 自己发射的子弹不走协议创建, 本地创建后再发送协议给服务器广播给其他客户端
			if (selfPlayerUnit.Id == unitId)
            {
				if (message.BulletId >= bulletLogicComponent.GetOneHitBulletIdFix())
					bulletLogicComponent.OneHitBulletIdStack.Push(message.BulletId);
				return;
            }

			FisheryComponent fisheryComponent = CurrentScene.GetComponent<FisheryComponent>();
			int seatId = fisheryComponent.GetSeatId(message.UnitId);

			UnitInfo UnitInfo = bulletLogicComponent.PopUnitInfo(seatId, message.BulletId, message.TrackFishUnitId);

			var publishData = ReceiveFire.Instance;
			publishData.CurrentScene = CurrentScene;
			publishData.UnitInfo = UnitInfo;
			publishData.TouchPosX = message.TouchPosX;
			publishData.TouchPosY = message.TouchPosY;
			publishData.Message = message;
			Game.EventSystem.PublishClass(publishData);
		}
	}
}