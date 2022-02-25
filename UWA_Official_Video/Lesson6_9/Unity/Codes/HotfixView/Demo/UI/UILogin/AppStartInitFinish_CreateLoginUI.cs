

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin, UILayer.Mid);

			Computer computer = args.ZoneScene.AddChild<Computer>();

			//Game.EventSystem.Publish(new EventType.InstallComputer(){Computer =  computer});

            Log.Debug("Before Publish TimeHelper.ClientNow() = " + TimeHelper.ClientNow());
			// 旧版 Coroutine 不会等待? 需要使用 await 关键字才会打到异步效果
			// 新版 直接使用 Coroutine 有异步效果?
			Game.EventSystem.PublishAsync(new EventType.InstallComputer(){Computer =  computer}).Coroutine();
			//await Game.EventSystem.PublishAsync(new EventType.InstallComputer(){Computer =  computer});
            Log.Debug("After Publish TimeHelper.ClientNow() = " + TimeHelper.ClientNow());
			computer.Start();
            Log.Debug("After Start TimeHelper.ClientNow() = " + TimeHelper.ClientNow());

            //computer.AddComponent<PCCaseComponent>();
            //computer.AddComponent<MonitorsComponent>();
            //computer.AddComponent<KeyBoardComponent>();
            //computer.AddComponent<MouseComponent>();

            //computer.Start();

            //await TimerComponent.Instance.WaitAsync(3000);

            //computer.Dispose();

            //UnitConfig config = UnitConfigCategory.Instance.Get(1001);

            //Log.Debug(config.Name);

            //var allUnitConfig = UnitConfigCategory.Instance.GetAll();

            //foreach (var unitConfig in allUnitConfig.Values)
            //         {
            //	Log.Debug(unitConfig.Name);
            //	Log.Debug(unitConfig.TestValue.ToString());
            //         }

            //UnitConfig heightConfig = UnitConfigCategory.Instance.GetUnitConfigByHeight(178);

            //Log.Debug(heightConfig.Name);

        }
	}
}
