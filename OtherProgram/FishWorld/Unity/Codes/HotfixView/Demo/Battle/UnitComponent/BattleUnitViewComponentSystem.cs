using UnityEngine;

namespace ET
{
    #region Life Circle

    [ObjectSystem]
    public class BattleUnitViewComponentAwakeSystem : AwakeSystem<BattleUnitViewComponent, Scene, Unit>
    {
        public override void Awake(BattleUnitViewComponent self, Scene currentScene, Unit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    self.NodeParent = GlobalComponent.Instance.FishRoot;
                    InitModel(currentScene, unit).Coroutine();
                    break;
                case UnitType.Bullet:

                    break;
            }
        }

        /// <summary>
        /// 初始化加载战斗用预设模型, 引用根节点 Transform, 这里传入加载用场景是因为
        /// Unit 的父场景不一定跟保存 GameObject 缓存池的父场景节点一致
        /// (缓存池在 ZoneScene 或者是 Current Scene)
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值, 这里异步加载实例化完后逻辑交由
        /// </summary>
        private async ETTask InitModel(Scene currentScene, Unit unit)
        {
            // Battle Warning 原逻辑将 Asset Bundle Name 加进 ObjectPool 里作为 Key, 而不是使用加载的 Asset Name
            // 如果是同一个 AssetBundle 里有不同的预设资源则会有问题, 目前模型资源是一个 AssetBundle 里一个预设
            // UI 资源可能多个预设在同一个 AssetBundle 里, ObjectPoolComponent 则不可使用
            // 使用 Asset Bundle Name 拼接 Asset Name 或者是别的方法
            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            string assetBundlePath = unitConfig.FishAssetBundlePath;
            string assetName = unitConfig.FishAssetName;

            ObjectPoolComponent objectPoolComponent = ViewComponentHelper.GetObjectPoolComponent(unit);
            GameObject gameObject = objectPoolComponent?.PopObject(assetBundlePath);

            if (gameObject == null)
                gameObject = await LoadModelPrefab(currentScene, assetBundlePath, assetName);

            if (gameObject == null)
            {
                string errorMsg = $"BattleUnitViewComponent.InitModel error assetBundlePath = { assetBundlePath }, assetName = { assetName }";
                throw new System.Exception(errorMsg);
            }

            Transform node = gameObject.transform;

            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            unit.AddComponent<GameObjectComponent, string, Transform>(assetBundlePath, node, isUseModelPool);
            unit.InitTransform();
        }

        /// <summary>
        /// 加载战斗用预设模型
        /// 原逻辑先走 AssetBundle 加载, 再调用对象池获取, 正常逻辑, 对象池没有走加载流程
        /// 判断 AssetBundle 是否加载, 没有则加载 ResourcesLoaderComponent.LoadAsync 完成了这一步, 直接调用即可
        /// 再实例化预设, 这里的加载 AssetBundle 跟获取 AssetBundle 里的资源都会抛出异常
        /// </summary>
        /// <param name = "currentScene" > 挂载加载组件的场景 </ param >
        /// <param name = "assetBundlePath" > AssetBundle 路径</param>
        /// <param name = "assetName" > AssetName 名</param>
        /// <returns>GameObject 实例化对象</returns>
        private async ETTask<GameObject> LoadModelPrefab(Scene currentScene, string assetBundlePath, string assetName)
        {
            try
            {
                ResourcesLoaderComponent resourcesLoaderCom = currentScene.GetComponent<ResourcesLoaderComponent>();
                await resourcesLoaderCom.LoadAsync(assetBundlePath);

                // Battle TODO 后面改用异步
                GameObject prefab = ResourcesComponent.Instance.GetAsset(assetBundlePath, assetName) as GameObject;

                return UnityEngine.Object.Instantiate(prefab);
            }
            catch (System.Exception exception)
            {
                Log.Error($"private Unit.LoadModelPrefab() exception msg = { exception.Message }");
            }

            return null;
        }
    }

    [ObjectSystem]
    public class BattleUnitViewComponentDestroySystem : DestroySystem<BattleUnitViewComponent>
    {
        public override void Destroy(BattleUnitViewComponent self) => self.NodeParent = null;
    }

    #endregion

    #region Base Function

    public static class UnitViewComponentSystem
    {
        public static void Update(this Unit self)
        {
            TransformComponent transformComponent = self.GetComponent<TransformComponent>();
            self.SetLocalPos(transformComponent.LogicLocalPos);
            self.SetForward(transformComponent.LogicForward);
        }
    }

    #endregion
}