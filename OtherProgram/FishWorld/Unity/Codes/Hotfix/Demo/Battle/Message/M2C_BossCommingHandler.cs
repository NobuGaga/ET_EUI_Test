using ET.EventType;

namespace ET
{
	/// <summary> 区域转换接收协议 </summary>
	[MessageHandler]
	public class M2C_BossCommingHandler : AMHandler<M2C_BossComming>
	{
		protected override async ETTask Run(Session session, M2C_BossComming message)
		{
			Scene zoneScene = session.DomainScene();
			Scene CurrentScene = zoneScene.CurrentScene();

			// Battle TODO
			ReceiveBossComming eventData = new ReceiveBossComming() {
				BossUnitConfigId = message.CfgId,
                ZoneScene        = zoneScene,
			};

			Game.EventSystem.Publish(eventData);

			await ETTask.CompletedTask;
		}
	}
}