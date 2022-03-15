using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class GameObjectComponentAwakeSystem : AwakeSystem<GameObjectComponent, string, Transform, Transform>
    {
        public override void Awake(GameObjectComponent self, string AssetName, Transform parent, Transform node)
        {
            self.AssetName = AssetName;
            self.Transform = node;
            node.parent = parent;
            node.localPosition = Vector3.zero;
        }
    }
}