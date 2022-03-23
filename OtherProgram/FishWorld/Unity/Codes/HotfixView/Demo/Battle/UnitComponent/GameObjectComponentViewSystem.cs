using UnityEngine;

namespace ET
{
    /// <summary>
    /// GameObject 生命周期跟随 ObjectPoolComponent(如果有使用的话)
    /// </summary>
    [ObjectSystem]
    public class GameObjectComponentAwakeSystem : AwakeSystem<GameObjectComponent, string, Transform>
    {
        public override void Awake(GameObjectComponent self, string AssetName, Transform node)
        {
            Unit unit = self.Parent as Unit;
            self.AssetName = AssetName;
            self.Transform = node;

            BattleUnitViewComponent battleUnitViewComponent = unit.GetComponent<BattleUnitViewComponent>();
            // 不是战斗相关的实体直接置为零点
            if (battleUnitViewComponent == null)
                node.localPosition = Vector3.zero;
        }
    }

    public static class GameObjectComponentViewSystem
    {
        public static ObjectPoolComponent ObjectPoolComponent(this GameObjectComponent self)
        {
            Unit unit = self.Parent as Unit;
            return ViewComponentHelper.GetObjectPoolComponent(unit);
        }
    }
}