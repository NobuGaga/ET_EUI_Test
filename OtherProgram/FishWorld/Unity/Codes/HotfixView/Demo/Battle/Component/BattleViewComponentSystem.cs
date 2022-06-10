using UnityEngine;
using ET.EventType;

using BattleMonoComponentExtension = ET.BattleMonoComponentSystem;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(SkillComponent))]
    [FriendClass(typeof(Unit))]
    //[FriendClass(typeof(FisheryViewComponentSystem))]
    public class BattleViewComponentAwakeSystem : AwakeSystem<BattleViewComponent>
    {
        public override void Awake(BattleViewComponent self)
        {
            // 初始化视图层 Foreach 方法
            var skillComponent = BattleLogicComponent.Instance.SkillComponent;
            skillComponent.SetFishAnimatorState = FisheryViewComponentSystem.SetFishAnimatorState;
            skillComponent.UpdateBeforeBullet = (Unit playerUnit) =>
                                                playerUnit.PlayerSkillComponent.UpdateBeforeBullet();

            BattleMonoComponent.Instance.EnterBattle(BattleMonoComponentExtension.Collide,
                                                     BattleMonoComponentExtension.RemoveFishUnit,
                                                     SkillLogicComponentSystem.FixedUpdateBeforeFish,
                                                     SkillViewHelper.UpdateBeforeBullet);

            BattleViewComponent.Instance = self;

            // 战斗用另外一个对象池组件, 生命周期跟战斗视图组件
            // 只用来管理鱼跟子弹, 不直接 Add 到 Scene 上是因为 CurrentScene 跟 ZoneScene 已经有了
            // 技能挂载在 UI 界面的节点由 UI 的 ObjectPoolComponent 管理
            // 挂载到非 UI 界面的节点由 BattleViewComponent 管理
            self.AddComponent<ObjectPoolComponent>();

            self.LastCreateFishTime = 0;
            self.AutoCreateFishGroupIndex = 0;
            self.C2M_GM = new C2M_GM() { Param = new System.Collections.Generic.List<string>() };

            self.Action_String_UnityObject = UnitViewAnimationSystem.ForeachBundleAsset;
        }
    }

    [ObjectSystem]
    public class BattleViewComponentDestroySystem : DestroySystem<BattleViewComponent>
    {
        public override void Destroy(BattleViewComponent self)
        {
            BattleMonoComponent.Instance.ExitBattle();
            BattleViewComponent.Instance = null;
            self.InstantiateFishStack.Clear();
        }
    }

    [ObjectSystem]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BulletLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    public class BattleViewComponentUpdateSystem : UpdateSystem<BattleViewComponent>
    {
        public override void Update(BattleViewComponent self)
        {
            FrameInstantiateObject(self);
            AutoCreateFish(self);
        }

        private void FrameInstantiateObject(BattleViewComponent self)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            if (currentScene == null) return;

            self.CurrentInstantiateCount = 0;
            var unitComponent = battleLogicComponent.UnitComponent;
            while (self.CurrentInstantiateCount < BattleConfig.FrameInstantiateObjectCount &&
                   self.InstantiateFishStack.Count > 0)
            {
                long fishUnitId = self.InstantiateFishStack.Pop();
                Unit fishUnit = unitComponent.Get(fishUnitId);
                if (fishUnit == null || fishUnit.IsDisposed)
                    continue;

                fishUnit.InstantiateGameObject();
                self.CurrentInstantiateCount++;
            }
        }

        private void AutoCreateFish(BattleViewComponent self)
        {
            if (!BattleConfig.IsAutoCreateFish) return;

            long currentServerTime = TimeHelper.ServerNow();
            if (self.LastCreateFishTime > 0 && currentServerTime - self.LastCreateFishTime < GMConfig.CreateFishInterval)
                return;

            self.LastCreateFishTime = currentServerTime;
            var fishBaseConfigIDList = GMConfig.FishBaseConfigIDList;
            ref ushort fishGroupID = ref fishBaseConfigIDList[self.AutoCreateFishGroupIndex];
            if (++self.AutoCreateFishGroupIndex >= fishBaseConfigIDList.Length)
                self.AutoCreateFishGroupIndex = 0;

            self.C2M_GM.Cmd = GMConfig.MakeFishGM;
            self.C2M_GM.Param.Clear();
            self.C2M_GM.Param.Add(fishGroupID.ToString());
            BattleLogicComponent.Instance.SessionComponent.Session.Send(self.C2M_GM);
        }
    }

    public class AfterCreateZoneScene_BattleViewComponent : AEvent<AfterCreateZoneScene>
    {
        protected override void Run(AfterCreateZoneScene args)
        {
            var objectPool = ObjectPool.Instance;
            for (int index = 0; index < ConstHelper.PreCreateFishClassCount; index++)
            {
                objectPool.Recycle(typeof(BattleUnitViewComponent), new BattleUnitViewComponent());
                objectPool.Recycle(typeof(GameObjectComponent), new GameObjectComponent());
                objectPool.Recycle(typeof(AnimatorComponent), new AnimatorComponent());
            }
            BattleMonoComponent.Instance.EnterGame();
        }
    }

    public class AfterCreateCurrentScene_BattleViewComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args)
        {
            if (args.CurrentScene.Name != "scene_home")
                args.CurrentScene.AddComponent<BattleViewComponent>();
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    public class InitTimeLine_BattleViewComponent : AEventClass<InitTimeLine>
    {
        protected override void Run(object obj)
        {
            var args = obj as InitTimeLine;
            var battleLogicComponent = BattleLogicComponent.Instance;
            Unit fishUnit = battleLogicComponent.UnitComponent.Get(args.UnitId);
            if (fishUnit != null && !fishUnit.IsDisposed)
                fishUnit.InitTimeLine(args.Info);
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    public class ExecuteTimeLine_BattleViewComponent : AEventClass<ExecuteTimeLine>
    {
        protected override void Run(object obj)
        {
            var args = obj as ExecuteTimeLine;
            var battleLogicComponent = BattleLogicComponent.Instance;
            Unit fishUnit = battleLogicComponent.UnitComponent.Get(args.UnitId);
            if (fishUnit != null && !fishUnit.IsDisposed)
                fishUnit.Execute(args.Info);
        }
    }

    public static class BattleViewComponentSystem
    {
        /// <summary> 预设预加载实例化 </summary>
        public static async ETTask PreLoadAndInstantiateObject(this BattleViewComponent self)
        {
            // Boss 资源 ID
            string redId = "fish_10101";
            var assetBundleData = UnitConfigCategory.Instance.GetAssetBundleData(redId);
            string assetBundlePath = assetBundleData.Path;
            string clipBundlePath = assetBundleData.ClipPath;

            var gameObject = await ObjectInstantiateHelper.LoadAsset(assetBundlePath, redId) as GameObject;
            gameObject = UnityEngine.Object.Instantiate(gameObject);
            gameObject.transform.parent = ReferenceHelper.FishRootNode.transform;
            gameObject.transform.localPosition = FishConfig.RemovePoint;
            gameObject.name = FishConfig.DefaultName;

            var objectPoolComponent = self.GetComponent<ObjectPoolComponent>();
            objectPoolComponent?.PushObject(assetBundlePath, gameObject);

            // Boss 动作资源
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            var ResourcesLoaderComponent = currentScene.GetComponent<ResourcesLoaderComponent>();
            await ResourcesLoaderComponent.LoadAsync(clipBundlePath);
        }
    }
}