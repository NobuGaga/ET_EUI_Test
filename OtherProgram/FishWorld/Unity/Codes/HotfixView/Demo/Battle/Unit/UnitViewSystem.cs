using UnityEngine;

namespace ET
{

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(BattleUnitViewComponent))]
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