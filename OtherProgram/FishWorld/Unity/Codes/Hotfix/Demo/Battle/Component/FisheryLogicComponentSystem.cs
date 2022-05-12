// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(Unit))]
    public sealed class FisheryComponentAwakeSystem : AwakeSystem<FisheryComponent>
    {
        public override void Awake(FisheryComponent self)
        {
            // Battle TODO 暂时写死
            self.RoomType = 1;

            self.IsMovingCamera = false;

            self.QuickMoveFish = (Unit fishUnit) =>
            {
                var fishUnitComponent = fishUnit.FishUnitComponent;
                fishUnitComponent.ResumeMove();
                fishUnitComponent.SetMoveSpeed(FishConfig.QuickMoveSpeed);
            };
        }
    }

    [ObjectSystem]
    public sealed class FisheryComponentDestroySystem : DestroySystem<FisheryComponent>
    {
        public override void Destroy(FisheryComponent self) => self.QuickMoveFish = null;
    }

    [FriendClass(typeof(PlayerComponent))]
    [FriendClass(typeof(FisheryComponent))]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class FisheryLogicComponentSystem
    {
        public static int GetSeatId(this FisheryComponent self, long playerUnitId)
        {
            UnitComponent unitComponent = BattleLogicComponent.Instance.UnitComponent;
            Unit playerUnit = unitComponent.Get(playerUnitId);
            NumericComponent numericComponent = playerUnit.GetComponent<NumericComponent>();
            return numericComponent.GetAsInt(NumericType.Pos);
        }

        public static int GetSelfSeatId(this FisheryComponent self)
        {
            var playerComponent = self.Parent.Parent.Parent.GetComponent<PlayerComponent>();
            return self.GetSeatId(playerComponent.MyId);
        }

        /// <summary> 渔场冰冻技能逻辑处理 </summary>
        /// <param name="isSkillStart">是否开始使用冰冻技能</param>
        internal static void FisheryIceSkill(this FisheryComponent self, bool isSkillStart)
        {
            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
            var fishUnitList = unitComponent.GetFishUnitList();
            BattleLogicComponent.Instance.Foreach(fishUnitList, SetFishUnitPauseState, isSkillStart);
        }
        
        private static void SetFishUnitPauseState(Unit fishUnit, bool isPause)
        {
            var fishUnitComponent = fishUnit.FishUnitComponent;
            fishUnitComponent.MoveInfo.IsPause = isPause;
        }

        /// <summary> 渔场切换场景处理 </summary>
        internal static void QuickMoveAllFish(this FisheryComponent self)
        {
            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
            if (unitComponent == null)
                return;

            var fishUnitList = unitComponent.GetFishUnitList();
            ForeachHelper.Foreach(fishUnitList, self.QuickMoveFish);
        }
    }
}