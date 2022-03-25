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
        private static int GetSeatId(this Unit playerUnit)
        {
            NumericComponent numericComponent = playerUnit.GetComponent<NumericComponent>();
            return numericComponent.GetAsInt(NumericType.Pos);
        }

        public static int GetSeatId(this FisheryComponent self, long playerUnitId)
        {
            Scene currentScene = self.Parent as Scene;
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit playerUnit = unitComponent.Get(playerUnitId);
            return playerUnit.GetSeatId();
        }

        public static int GetSelfSeatId(this FisheryComponent self)
        {
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Parent as Scene);
            return selfPlayerUnit.GetSeatId();
        }

        public static CannonComponent GetSelfCannonComponent(this FisheryComponent self)
        {
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Parent as Scene);
            return selfPlayerUnit.GetComponent<CannonComponent>();
        }
    }

    #endregion
}