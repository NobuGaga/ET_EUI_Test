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
                self.NodeParent = ReferenceHelper.FishRootNode.transform;
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
            var ret = TryGetAssetPathAndName(unit);
            self.AssetBundlePath = ret.Item1;
            string assetName = ret.Item2;

            var objectPoolComponent = unit.GetObjectPoolComponent();
            GameObject gameObject = objectPoolComponent?.PopObject(self.AssetBundlePath);

            if (gameObject != null)
            {
                unit.InitViewComponent(gameObject);
                return;
            }

            gameObject = await ObjectInstantiateHelper.LoadModelPrefab(self.AssetBundlePath, assetName);
            self.PrefabObject = gameObject;

            // Unit 已经被释放掉
            if (gameObject == null || unit == null || unit.IsDisposed)
                return;

            if (unit.Type == UnitType.Fish)
                BattleViewComponent.Instance.InstantiateFishStack.Push(unit.UnitId);
            else if (unit.Type == UnitType.Bullet)
                unit.InstantiateGameObject();
        }

        private ValueTuple<string, string> TryGetAssetPathAndName(Unit unit)
        {
            string assetBundlePath = ABPath.cannon_1AB;
            string assetName = "cannon_1";
            switch (unit.Type)
            {
                case UnitType.Fish:
                    try
                    {
                        // Battle TODO 暂时只有鱼读表, 后续将子弹也读表
                        UnitConfig unitConfig = unit.Config;
                        assetBundlePath = unitConfig.FishAssetBundlePath;
                        assetName = unitConfig.FishAssetName;
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"private Unit.TryGetAssetPathAndName() exception msg = {exception.Message}");
                    }
                    break;
                case UnitType.Bullet:
                    assetBundlePath = ABPath.cannon_1_bulletAB;
                    assetName = "cannon_1_bullet";
                    break;
            }

            return new ValueTuple<string, string>(assetBundlePath, assetName);
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
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(BattleUnitViewComponent))]
    public static class UnitViewSystem
    {
        internal static void InstantiateGameObject(this Unit self)
        {
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            var gameObject = UnityEngine.Object.Instantiate(battleUnitViewComponent.PrefabObject);
            self.InitViewComponent(gameObject);
        }

        internal static void InitViewComponent(this Unit self, GameObject gameObject)
        {
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            Transform node = gameObject.transform;
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            self.GameObjectComponent = self.AddComponent<GameObjectComponent, string, Transform>(
                                                         battleUnitViewComponent.AssetBundlePath, node, isUseModelPool);

            self.InitTransform();
            BattleMonoUnit monoUnit = UnitMonoComponent.Instance.Get(self.UnitId);
            monoUnit.ColliderMonoComponent = ColliderHelper.AddColliderComponent(self.ConfigId, gameObject);

            if (self.Type == UnitType.Fish)
                self.AddComponent<AnimatorComponent>(BattleConfig.IsUseModelPool).Reset();

            TransformMonoHelper.Add(self.Id, node);
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