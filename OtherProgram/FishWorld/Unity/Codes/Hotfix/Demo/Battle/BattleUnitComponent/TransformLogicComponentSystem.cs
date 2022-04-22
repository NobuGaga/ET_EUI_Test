// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    public class TransformComponentAwakeSystem : AwakeSystem<TransformComponent>
    {
        public override void Awake(TransformComponent self)
        {
            self.Info = TransformHelper.PopInfo();
            self.Info.Reset();
            self.NodeName = TransformDefaultConfig.DefaultName;
        }
    }

    [ObjectSystem]
    public class TransformComponentDestroySystem : DestroySystem<TransformComponent>
    {
        public override void Destroy(TransformComponent self)
        {
            TransformHelper.PushPool(self.Info);
            self.Info = null;
            self.NodeName = null;
        }
    }
}