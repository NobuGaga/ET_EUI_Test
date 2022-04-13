// Battle Review Before Boss Node

namespace ET
{
	/// <summary> 区域转换接收协议 </summary>
	[MessageHandler]
	public class M2C_BossCommingHandler : AMHandler<M2C_BossComming>
	{
		protected override async ETTask Run(Session session, M2C_BossComming message)
		{
			Game.EventSystem.Publish(new EventType.ReceiveBossComming()
			{
				ZoneScene = session.DomainScene(),
				BossUnitConfigId = message.CfgId,
			});

			await ETTask.CompletedTask;
		}
	}
}