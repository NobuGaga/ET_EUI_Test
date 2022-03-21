using UnityEngine;

namespace ET
{
    /// <summary> 每个 Unit 通用都有的视图引用或者组件都在这里设置 </summary>
    public static class UnitViewSystem
    {
        public static BattleUnitViewComponent BattleUnitViewComponent(this Unit self)
                                => self.GetComponent<BattleUnitViewComponent>();

        /// <summary>
        /// 初始化加载战斗用预设模型, 引用根节点 Transform, 这里传入加载用场景是因为
        /// Unit 的父节点不一定跟保存 GameObject 的场景节点一致
        /// (GameObject 缓存在 ZoneScene 或者是 Current Scene)
        /// 不放在 IAake 流程实现因为要走统一事件流程调用
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值, 这里异步加载实例化完后逻辑交由
        /// TransformComponent 组件管理其更新生命周期
        /// </summary>
        /// <param name="self">单位(战斗)</param>
        /// <param name="currentScene">挂载加载组件的场景</param>
        /// <param name="assetBundlePath">AssetBundle 路径</param>
        /// <param name="assetBundleName">AssetBundle 名</param>
        /// <returns></returns>
        public static async ETTask InitModel(this Unit self, Scene currentScene, string assetBundlePath, string assetBundleName)
        {
            // TODO Battle Delete
            if (currentScene.Name == "scene_home")
                return;

            // TODO Battle Delete
            if (self.UnitType != UnitType.Fish && self.UnitType != UnitType.Bullet)
                return;

            // 同步增加 BattleUnitViewComponent, 在创建完 Unit 后
            self.AddComponent<BattleUnitViewComponent>();
            // Battle Warning 原逻辑将 Asset Bundle Name 加进 ObjectPool 里作为 Key, 而不是使用加载的 Asset Name
            // 如果是同一个 AssetBundle 里有不同的预设资源则会有问题, 目前模型资源是一个 AssetBundle 里一个预设
            // UI 资源可能多个预设在同一个 AssetBundle 里, ObjectPoolComponent 则不可使用
            // 使用 Asset Bundle Name 拼接 Asset Name 或者是别的方法
            ObjectPoolComponent objectPoolComponent = ViewComponentHelper.GetObjectPoolComponent(self);
            GameObject gameObject = objectPoolComponent?.PopObject(assetBundlePath);

            if (gameObject == null)
                gameObject = await LoadModelPrefab(currentScene, assetBundlePath, assetBundleName);

            if (gameObject == null)
            {
                string errorMsg = $"Unit.InitModel error assetBundlePath = { assetBundlePath }, assetBundleName = { assetBundleName }";
                throw new System.Exception(errorMsg);
            }

            Transform node = gameObject.transform;
            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            self.AddComponent<GameObjectComponent, string, Transform>(assetBundlePath, node, isUseModelPool);
            Game.EventSystem.Publish(new EventType.AfterBattleGameObjectCreate() { Unit = self });
        }

        /// <summary>
        /// 加载战斗用预设模型
        /// 原逻辑先走 AssetBundle 加载, 再调用对象池获取, 正常逻辑, 对象池没有走加载流程
        /// 判断 AssetBundle 是否加载, 没有则加载 ResourcesLoaderComponent.LoadAsync 完成了这一步, 直接调用即可
        /// 再实例化预设, 这里的加载 AssetBundle 跟获取 AssetBundle 里的资源都会抛出异常
        /// </summary>
        /// <param name = "currentScene" > 挂载加载组件的场景 </ param >
        /// <param name = "assetBundlePath" > AssetBundle 路径</param>
        /// <param name = "assetBundleName" > AssetName 名</param>
        /// <returns>GameObject 实例化对象</returns>
        private static async ETTask<GameObject> LoadModelPrefab(Scene currentScene, string assetBundlePath, string assetBundleName)
        {
            try
            {
                ResourcesLoaderComponent resourcesLoaderCom = currentScene.GetComponent<ResourcesLoaderComponent>();
                await resourcesLoaderCom.LoadAsync(assetBundlePath);
                // TODO 后面改用异步
                GameObject prefab = ResourcesComponent.Instance.GetAsset(assetBundlePath, assetBundleName) as GameObject;
                return UnityEngine.Object.Instantiate(prefab);
            }
            catch (System.Exception exception)
            {
                Log.Error($"private Unit.LoadModelPrefab() exception msg = { exception.Message }");
            }
            return null;
        }
    }
}