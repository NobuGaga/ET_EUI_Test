using System;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class BattleUnitViewComponentAwakeSystem : AwakeSystem<BattleUnitViewComponent, Scene, Unit>
    {
        public override void Awake(BattleUnitViewComponent self, Scene currentScene, Unit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    self.NodeParent = ReferenceHelper.FishRootNode.transform;
                    break;
                case UnitType.Bullet:
                    self.NodeParent = ReferenceHelper.BulletRootNode.transform;
                    break;
            }

            InitModel(currentScene, unit).Coroutine();
        }

        /// <summary>
        /// 初始化加载战斗用预设模型, 引用根节点 Transform, 这里传入加载用场景是因为
        /// Unit 的父场景不一定跟保存 GameObject 缓存池的父场景节点一致
        /// (缓存池在 ZoneScene 或者是 Current Scene)
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值
        /// </summary>
        private async ETTask InitModel(Scene currentScene, Unit unit)
        {
            // Battle Warning 原逻辑将 Asset Bundle Name 加进 ObjectPool 里作为 Key, 而不是使用加载的 Asset Name
            // 如果是同一个 AssetBundle 里有不同的预设资源则会有问题, 目前模型资源是一个 AssetBundle 里一个预设
            // UI 资源可能多个预设在同一个 AssetBundle 里, ObjectPoolComponent 则不可使用
            // 使用 Asset Bundle Name 拼接 Asset Name 或者是别的方法

            var ret = TryGetAssetPathAndName(unit);
            string assetBundlePath = ret.Item1;
            string assetName = ret.Item2;

            ObjectPoolComponent objectPoolComponent = unit.GetObjectPoolComponent();
            GameObject gameObject = objectPoolComponent?.PopObject(assetBundlePath);

            // 这里会抛出异常
            if (gameObject == null)
                gameObject = await ObjectInstantiateHelper.InitModel(currentScene, unit, assetBundlePath, assetName);

            // Unit 已经被释放掉
            if (gameObject == null)
                return;

            Transform node = gameObject.transform;
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            unit.AddComponent<GameObjectComponent, string, Transform>(assetBundlePath, node, isUseModelPool);
            unit.InitTransform();
            InitComponent(unit);
        }

        private ValueTuple<string, string> TryGetAssetPathAndName(Unit unit)
        {
            string assetBundlePath = ABPath.cannon_1AB;
            string assetName = "cannon_1";
            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    try
                    {
                        // Battle TODO 暂时只有鱼读表, 后续将子弹也读表
                        UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
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

        private void InitComponent(Unit unit)
        {
            unit.AddComponent<ColliderViewComponent>();

            if (unit.UnitType == UnitType.Bullet)
                return;

            unit.AddComponent<AnimatorComponent>(BattleConfig.IsUseModelPool).Reset();
        }
    }

    [ObjectSystem]
    public class BattleUnitViewComponentDestroySystem : DestroySystem<BattleUnitViewComponent>
    {
        public override void Destroy(BattleUnitViewComponent self) => self.NodeParent = null;
    }

    public static class UnitViewComponentSystem
    {
        public static void Update(this Unit self)
        {
            TransformComponent transformComponent = self.GetComponent<TransformComponent>();
            self.SetLocalPos(transformComponent.Info.LogicLocalPos);
            switch (self.UnitType)
            {
                case UnitType.Fish:
                    self.SetForward(transformComponent.Info.LogicForward);
                    break;
                case UnitType.Bullet:
                    self.SetLocalRotation(transformComponent.Info.LogicLocalRotation);
                    break;
            }

            self.UpdateScreenPosition();

            ColliderViewComponent colliderViewComponent = self.GetComponent<ColliderViewComponent>();
            colliderViewComponent?.Update();
        }

        public static ObjectPoolComponent GetObjectPoolComponent(this Unit unit)
        {
            Scene currentScene = unit.DomainScene();
            UnitType unitType = unit.UnitType;
            if (unitType == UnitType.Player || unitType == UnitType.Player)
                return currentScene.GetComponent<ObjectPoolComponent>();

            BattleViewComponent battleViewComponent = currentScene.GetBattleViewComponent();
            return battleViewComponent.GetComponent<ObjectPoolComponent>();
        }
    }
}