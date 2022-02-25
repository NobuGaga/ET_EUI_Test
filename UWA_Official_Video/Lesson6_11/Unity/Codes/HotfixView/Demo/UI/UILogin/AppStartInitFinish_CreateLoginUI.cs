using System.Numerics;

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(args.ZoneScene, UIType.UILogin, UILayer.Mid);

			//Computer computer = args.ZoneScene.AddChild<Computer>();

			//Game.EventSystem.Publish(new EventType.InstallComputer(){Computer =  computer});

			//Log.Debug("Before Publish TimeHelper.ClientNow() = " + TimeHelper.ClientNow());
			// 旧版 Coroutine 不会等待? 需要使用 await 关键字才会打到异步效果
			// 新版 直接使用 Coroutine 有异步效果?
			//Game.EventSystem.PublishAsync(new EventType.InstallComputer() { Computer = computer }).Coroutine();
			//await Game.EventSystem.PublishAsync(new EventType.InstallComputer(){Computer =  computer});
			//Log.Debug("After Publish TimeHelper.ClientNow() = " + TimeHelper.ClientNow());
			//computer.Start();
			//Log.Debug("After Start TimeHelper.ClientNow() = " + TimeHelper.ClientNow());

			//computer.AddComponent<PCCaseComponent>();
   //         computer.AddComponent<MonitorsComponent>();
   //         computer.AddComponent<KeyBoardComponent>();
   //         computer.AddComponent<MouseComponent>();

   //         computer.Start();

   //         await TimerComponent.Instance.WaitAsync(3000);

   //         computer.Dispose();

   //         UnitConfig config = UnitConfigCategory.Instance.Get(1001);

   //         Log.Debug(config.Name);

   //         var allUnitConfig = UnitConfigCategory.Instance.GetAll();

			//foreach (var unitConfig in allUnitConfig.Values)
   //         {
   //             Log.Debug(unitConfig.Name);
   //             Log.Debug(unitConfig.TestValue.ToString());
   //         }
   //         UnitConfig heightConfig = UnitConfigCategory.Instance.GetUnitConfigByHeight(178);

			//Log.Debug(heightConfig.Name);

            Log.Debug("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			// 使用 Coroutine 直接返回
			//this.TestAsync().Coroutine();
			// 使用 await 等待返回
			//await this.TestAsync();
			// 有返回结果的必须使用 await 需要等待返回结果
			//int result = await this.TestResultAsync();

			//Log.Debug(result.ToString());

			ETCancellationToken cancellationToken = new ETCancellationToken();
			MoveToAsync(Vector3.Zero, cancellationToken).Coroutine();
            Log.Debug("bbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
			cancellationToken.Cancel();
		}

		public async ETTask TestAsync()
        {
			Log.Debug("1111111111111111111111111111");
			await TimerComponent.Instance.WaitAsync(2000);
			Log.Debug("2222222222222222222222222222");
			// ETTask.CompletedTask 变为同步
			//await ETTask.CompletedTask;
		}

		public async ETTask<int> TestResultAsync()
		{
			Log.Debug("1111111111111111111111111111");
			await TimerComponent.Instance.WaitAsync(2000);
			Log.Debug("2222222222222222222222222222");

			return 10;
		}

		public async ETTask MoveToAsync(Vector3 pos, ETCancellationToken cancellationToken)
        {
			Log.Debug("Move Start!!!");
			bool ret = await TimerComponent.Instance.WaitAsync(3000, cancellationToken);
		
			if (ret)
            {
				Log.Debug("Move Over!!!");

			}
			else
            {
				Log.Debug("Move Stop!!!");
			}

		}
	}
}
