using UnityEngine;

namespace ET
{
    /// <summary> GameObject 生命周期跟随 ObjectPoolComponent(如果有使用的话) </summary>
    [ObjectSystem]
    public class GameObjectComponentAwakeSystem : AwakeSystem<GameObjectComponent, string, Transform>
    {
        public override void Awake(GameObjectComponent self, string AssetName, Transform node)
        {
            self.AssetName = AssetName;
            self.Transform = node;

            if (self.Parent is Unit)
                AwakeUnit(self, self.Parent as Unit);
        }

        private void AwakeUnit(GameObjectComponent self, Unit unit)
        {
            BattleUnitViewComponent battleUnitViewComponent = unit.GetComponent<BattleUnitViewComponent>();

            // 不是战斗相关的实体直接置为零点
            if (battleUnitViewComponent == null)
                self.Transform.localPosition = Vector3.zero;
        }
    }

    public static class GameObjectComponentViewSystem
    {
        public static ObjectPoolComponent ObjectPoolComponent(this GameObjectComponent self)
        {
            Unit unit = self.Parent as Unit;
            return unit.GetObjectPoolComponent();
        }
    }
}