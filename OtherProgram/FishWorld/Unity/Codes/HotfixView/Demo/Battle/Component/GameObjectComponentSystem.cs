using UnityEngine;

namespace ET
{
    /// <summary>
    /// GameObjectComponent 生命周期跟随 ObjectPoolComponent(如果有使用的话)
    /// </summary>
    [ObjectSystem]
    public class GameObjectComponentAwakeSystem : AwakeSystem<GameObjectComponent, string, Transform>
    {
        public override void Awake(GameObjectComponent self, string AssetName, Transform node)
        {
            Unit unit = self.Parent as Unit;
            self.AssetName = AssetName;
            self.Transform = node;

            BattleUnitViewComponent battleUnitViewComponent = unit.BattleUnitViewComponent();
            if (battleUnitViewComponent == null)
                node.localPosition = Vector3.zero;
        }
    }
}