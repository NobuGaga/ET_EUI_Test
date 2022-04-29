// Battle Review Before Boss Node

namespace ET
{
	/// <summary> 区域转换接收协议 </summary>
	[MessageHandler]
	[FriendClass(typeof(BattleLogicComponent))]
	public class M2C_BossCommingHandler : AMHandler<M2C_BossComming>
	{
		protected override void Run(Session session, M2C_BossComming message)
		{
			Game.EventSystem.Publish(new EventType.ReceiveBossComming()
			{
				ZoneScene = BattleLogicComponent.Instance.ZoneScene,
				BossUnitConfigId = message.CfgId,
			});
		}
	}
}