using System.Collections.Generic;
using ET.EventType;

namespace ET
{
	/// <summary> 发射子弹接收协议 </summary>
	[MessageHandler]
	public class M2C_FireHandler : AMHandler<M2C_Fire>
	{
		protected override async ETTask Run(Session session, M2C_Fire message)
		{
			Scene zoneScene = session.DomainScene();
			Scene CurrentScene = zoneScene.CurrentScene();

			BattleLogicComponent battleLogicComponent = CurrentScene.GetBattleLogicComponent();
			long unitId = message.UnitId;
			// 自己发射的子弹不走协议创建, 本地创建后再发送协议给服务器广播给其他客户端
			if (battleLogicComponent.GetSelfUnitId() == unitId)
				return;

			FisheryComponent fisheryComponent = CurrentScene.GetComponent<FisheryComponent>();

			int seatId = fisheryComponent.GetSeatId(message.UnitId);

			long trackFishUnitId = BulletConfig.DefaultTrackUnitId;
			if (message.FishId != null && message.FishId.Count > 0)
				trackFishUnitId = message.FishId[0];

			UnitInfo UnitInfo = new UnitInfo()
			{
				Ks = new List<int>() { NumericType.Pos, NumericType.TrackFishId, NumericType.BulletId },
				Vs = new List<long>() { seatId, trackFishUnitId, message.BulletId },
			};

			AfterShoot eventData = new AfterShoot()
			{
				CurrentScene = CurrentScene,
				UnitInfo = UnitInfo,
				ShootDirX = message.PosX,
				ShootDirY = message.PosY,
			};

			Game.EventSystem.Publish(eventData);

			await ETTask.CompletedTask;
		}
	}
}