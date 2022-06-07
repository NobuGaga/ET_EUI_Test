using UnityEngine;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    public class BattleUnitViewComponentAwakeSystem : AwakeSystem<BattleUnitViewComponent>
    {
        public override void Awake(BattleUnitViewComponent self)
        {
            Unit unit = self.Parent as Unit;
            var transformComponent = unit.TransformComponent;
            unit.BattleUnitViewComponent = self;

            if (unit.Type == UnitType.Fish)
            {
                if (ConstHelper.IsEditor)
                {
                    string unitIdName = (unit.UnitId % 100).ToString();
                    var lineId = unit.AttributeComponent[NumericType.RoadId];
                    transformComponent.NodeName = string.Format(FishConfig.NameFormat, unit.ConfigId, unit.Config.ResId, lineId, unitIdName);
                }

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
}