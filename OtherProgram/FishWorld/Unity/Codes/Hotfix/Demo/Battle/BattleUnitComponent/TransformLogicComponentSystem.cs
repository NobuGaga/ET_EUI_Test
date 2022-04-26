// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    public class TransformComponentAwakeSystem : AwakeSystem<TransformComponent>
    {
        public override void Awake(TransformComponent self)
        {
            self.Info = TransformInfoHelper.PopInfo(self.Parent.Id);
            self.Info.Reset();

            self.NodeName = TransformDefaultConfig.DefaultName;
        }
    }

    [ObjectSystem]
    public class TransformComponentDestroySystem : DestroySystem<TransformComponent>
    {
        public override void Destroy(TransformComponent self)
        {
            TransformInfo info = self.Info;
            self.Info = null;
            TransformInfoHelper.PushPool(self.Parent.Id, info);

            self.NodeName = null;
        }
    }
}