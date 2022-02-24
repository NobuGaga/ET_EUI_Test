

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin, UILayer.Mid);

			Computer computer = args.ZoneScene.AddChild<Computer>();

            computer.AddComponent<PCCaseComponent>();
            computer.AddComponent<MonitorsComponent>();
            computer.AddComponent<KeyBoardComponent>();
            computer.AddComponent<MouseComponent>();

			computer.Start();

			await TimerComponent.Instance.WaitAsync(3000);

			computer.Dispose();

			UnitConfig config = UnitConfigCategory.Instance.Get(1001);

			Log.Debug(config.Name);

			var allUnitConfig = UnitConfigCategory.Instance.GetAll();

			foreach (var unitConfig in allUnitConfig.Values)
            {
				Log.Debug(unitConfig.Name);
				Log.Debug(unitConfig.TestValue.ToString());
            }

			UnitConfig heightConfig = UnitConfigCategory.Instance.GetUnitConfigByHeight(178);

			Log.Debug(heightConfig.Name);

		}
	}
}
