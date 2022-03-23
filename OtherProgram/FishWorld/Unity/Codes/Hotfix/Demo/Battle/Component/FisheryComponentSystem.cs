using ET.EventType;

namespace ET
{
    #region Event

    public class AfterCreateCurrentScene_FisheryComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(AfterCreateCurrentScene args)
        {
            args.CurrentScene.AddComponent<FisheryComponent>();
            await ETTask.CompletedTask;
        }
    }

    #endregion

    #region Life Circle

    [ObjectSystem]
    public sealed class FisheryComponentAwakeSystem : AwakeSystem<FisheryComponent>
    {
        public override void Awake(FisheryComponent self)
        {
            // Battle TODO 暂时写死
            self.RoomType = 1;
        }
    }
    
    #endregion

    #region Base

    public static class FisheryComponentSystem
    {
        public static void Test(this FisheryComponent self)
        {
            // Battle TODO
        }
    }

    #endregion
}