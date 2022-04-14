using ET.EventType;

namespace ET
{
	/// <summary> 发射子弹接收协议 </summary>
	[MessageHandler]
	public class M2C_FireHandler : AMHandler<M2C_Fire>
	{
		protected override void Run(Session session, M2C_Fire message)
		{
			Scene zoneScene = session.DomainScene();
			Scene CurrentScene = zoneScene.CurrentScene();
			BulletLogicComponent bulletLogicComponent = CurrentScene.GetComponent<BulletLogicComponent>();

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

			Game.EventSystem.Publish(new ReceiveFire()
			{
				CurrentScene = CurrentScene,
				UnitInfo = UnitInfo,
				TouchPosX = message.TouchPosX,
				TouchPosY = message.TouchPosY,
				Message = message,
			});
		}
	}
}