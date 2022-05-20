using System;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(GameObjectComponent))]
    public class BattleUnitViewComponentAwakeSystem : AwakeSystem<BattleUnitViewComponent>
    {
        public override void Awake(BattleUnitViewComponent self)
        {
            Unit unit = self.Parent as Unit;
            unit.BattleUnitViewComponent = self;

            if (unit.Type == UnitType.Fish)
            {
                self.NodeParent = ReferenceHelper.FishRootNode.transform;
                self.MotionName = MotionTypeHelper.Get(AnimatorConfig.DefaultMotionType);
            }
            else if (unit.Type == UnitType.Bullet)
                self.NodeParent = ReferenceHelper.BulletRootNode.transform;

            InitModel(self, unit).Coroutine();
        }

        /// <summary>
        /// 初始化加载战斗用预设模型, 引用根节点 Transform, 这里传入加载用场景是因为
        /// Unit 的父场景不一定跟保存 GameObject 缓存池的父场景节点一致
        /// (缓存池在 ZoneScene 或者是 Current Scene)
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值
        /// </summary>
        private async ETTask InitModel(BattleUnitViewComponent self, Unit unit)
        {
            // Battle Warning 原逻辑将 Asset Bundle Name 加进 ObjectPool 里作为 Key, 而不是使用加载的 Asset Name
            // 如果是同一个 AssetBundle 里有不同的预设资源则会有问题, 目前模型资源是一个 AssetBundle 里一个预设
            // UI 资源可能多个预设在同一个 AssetBundle 里, ObjectPoolComponent 则不可使用
            // 使用 Asset Bundle Name 拼接 Asset Name 或者是别的方法
            var assetBundleData = TryGetAssetPathAndName(unit);
            self.AssetBundlePath = assetBundleData.Path;
            string assetName = assetBundleData.Name;

            var objectPoolComponent = unit.GetObjectPoolComponent();
            GameObject gameObject = objectPoolComponent?.PopObject(self.AssetBundlePath);

            if (gameObject != null)
            {
                unit.InitViewComponent(gameObject, self.AssetBundlePath);
                return;
            }

            gameObject = await ObjectInstantiateHelper.LoadAsset(self.AssetBundlePath, assetName) as GameObject;
            self.PrefabObject = gameObject;

            // Unit 已经被释放掉
            if (gameObject == null || unit == null || unit.IsDisposed)
                return;

            if (unit.Type == UnitType.Fish)
                BattleViewComponent.Instance.InstantiateFishStack.Push(unit.UnitId);
            else if (unit.Type == UnitType.Bullet)
                unit.InstantiateGameObject();
        }

        private AssetBundleData TryGetAssetPathAndName(Unit unit)
        {
            if (unit.Type == UnitType.Fish)
            {
                // Battle TODO 暂时只有鱼读表, 后续将子弹也读表
                var unitConfig = unit.Config;
                return UnitConfigCategory.Instance.GetAssetBundleData(unitConfig.ResId);
            }

            return new AssetBundleData()
            {
                Path = ABPath.cannon_1_bulletAB,
                Name = "cannon_1_bullet",
            };
        }
    }

    [ObjectSystem]
    public class BattleUnitViewComponentDestroySystem : DestroySystem<BattleUnitViewComponent>
    {
        public override void Destroy(BattleUnitViewComponent self)
        {
            self.NodeParent = null;
            self.AssetBundlePath = null;
            self.PrefabObject = null;
            self.MotionName = null;
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(BattleUnitViewComponent))]
    [FriendClass(typeof(AnimatorComponent))]
    public static class UnitViewSystem
    {
        internal static void InstantiateGameObject(this Unit self)
        {
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            var gameObject = UnityEngine.Object.Instantiate(battleUnitViewComponent.PrefabObject);
            self.InitViewComponent(gameObject, battleUnitViewComponent.AssetBundlePath);
        }

        internal static void InitViewComponent(this Unit self, GameObject gameObject, string assetBundlePath)
        {
            Transform node = gameObject.transform;
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            self.GameObjectComponent = self.AddComponent<GameObjectComponent, string, Transform>
                                                        (assetBundlePath, node, isUseModelPool);

            self.InitTransform();
            BattleMonoUnit monoUnit = UnitMonoComponent.Instance.Get(self.UnitId);
            monoUnit.ColliderMonoComponent = ColliderHelper.AddColliderComponent(self.ConfigId, gameObject);
            TransformMonoHelper.Add(self.UnitId, node);

            if (self.Type == UnitType.Fish)
                self.InitFishAnimator().Coroutine();
        }

        private static async ETTask InitFishAnimator(this Unit self)
        {
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            int configId = self.ConfigId;
            string resId = self.Config.ResId;
            var animatorComponent = self.AddComponent<AnimatorComponent, int>(configId, isUseModelPool);
            animatorComponent.Reset();
            self.AnimatorComponent = animatorComponent;

            if (AnimationClipHelper.Contains(resId))
                return;

            var assetBundleData = UnitConfigCategory.Instance.GetAssetBundleData(self.Config.ResId);
            string clipBundlePath = assetBundleData.ClipPath;
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            var ResourcesLoaderComponent = currentScene.GetComponent<ResourcesLoaderComponent>();
            await ResourcesLoaderComponent.LoadAsync(clipBundlePath);

            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            string motionName = battleUnitViewComponent.MotionName;
            if (AnimationClipHelper.Contains(resId))
            {
                if (!string.IsNullOrEmpty(motionName))
                    self.PlayAnimation(motionName, true);
                return;
            }

            var assetMap = ResourcesComponent.Instance.GetBundleAll(clipBundlePath);
            BattleLogicComponent.Instance.Argument_String = resId;

            // 将每个模型动作存到 Mono 层, 新的模型加载后只执行一次
            ForeachHelper.Foreach(assetMap, BattleViewComponent.Instance.Action_String_UnityObject);
            if (!string.IsNullOrEmpty(motionName))
                self.PlayAnimation(motionName, true);
        }

        internal static void ForeachBundleAsset(string assetName, UnityEngine.Object asset)
        {
            string resId = BattleLogicComponent.Instance.Argument_String;
            AnimationClipHelper.Add(resId, assetName, asset as AnimationClip);
        }

        internal static void PlayAnimation(this Unit self, int motionType, bool isLoop) =>
                             self.PlayAnimation(MotionTypeHelper.Get(motionType), isLoop);

        internal static void PlayAnimation(this Unit self, string motionName, bool isLoop)
        {
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;

            // 这里只储存循环播放的动作名, 用于作异步加载完后播放
            if (isLoop) battleUnitViewComponent.MotionName = motionName;

            if (self.GameObjectComponent == null) return;

            string resId = self.Config.ResId;
            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Play(gameObjectComponent.GameObject, resId, motionName, isLoop);

            if (isLoop) return;

            var clip = AnimationClipHelper.GetClip(resId, motionName);
            if (clip != null)
                self.ResumeMainMotion(clip.length).Coroutine();
        }

        internal static async ETTask ResumeMainMotion(this Unit self, float time)
        {
            await TimerComponent.Instance.WaitAsync((long)(time * 1000));
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            self.PlayAnimation(battleUnitViewComponent.MotionName, true);
        }

        internal static void PauseAnimation(this Unit self)
        {
            if (self.GameObjectComponent == null)
                return;

            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Pause(gameObjectComponent.GameObject);
        }

        internal static void StopAnimation(this Unit self)
        {
            if (self.GameObjectComponent == null)
                return;

            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Stop(gameObjectComponent.GameObject);
        }

        internal static ObjectPoolComponent GetObjectPoolComponent(this Unit unit)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            int unitType = unit.Type;
            if (unitType == UnitType.Player || unitType == UnitType.Cannon)
            {
                Scene currentScene = battleLogicComponent.CurrentScene;
                return currentScene.GetComponent<ObjectPoolComponent>();
            }

            var battleViewComponent = BattleViewComponent.Instance;
            return battleViewComponent.GetComponent<ObjectPoolComponent>();
        }
    }
}