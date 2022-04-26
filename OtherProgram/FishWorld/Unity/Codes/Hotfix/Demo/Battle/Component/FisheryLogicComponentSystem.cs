// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    public sealed class FisheryComponentAwakeSystem : AwakeSystem<FisheryComponent>
    {
        public override void Awake(FisheryComponent self)
        {
            // Battle TODO 暂时写死
            self.RoomType = 1;
        }
    }

    [FriendClass(typeof(PlayerComponent))]
    [FriendClass(typeof(FisheryComponent))]
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class FisheryLogicComponentSystem
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
            var playerComponent = self.Parent.Parent.Parent.GetComponent<PlayerComponent>();
            return self.GetSeatId(playerComponent.MyId);
        }

        public static Unit GetPlayerUnit(this FisheryComponent self, int seatId)
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            UnitComponent unitComponent = battleLogicComponent.UnitComponent;
            
            var playerUnitList = unitComponent.GetPlayerUnitList();
            battleLogicComponent.Result_Unit = null;

            battleLogicComponent.Foreach(playerUnitList, SetFishUnitPauseState, seatId);

            return battleLogicComponent.Result_Unit;
        }

        private static bool SetFishUnitPauseState(Unit playerUnit, int seatId)
        {
            var attributeComponent = playerUnit.GetComponent<NumericComponent>();
            if (attributeComponent.GetAsInt(NumericType.Pos) != seatId)
                return true;

            var battleLogicComponent = BattleLogicComponent.Instance;
            battleLogicComponent.Result_Unit = playerUnit;
            return false;
        }

        /// <summary> 渔场冰冻技能逻辑处理 </summary>
        /// <param name="isSkillStart">是否开始使用冰冻技能</param>
        internal static void FisheryIceSkill(this FisheryComponent self, bool isSkillStart)
        {
            UnitComponent unitComponent = self.Parent.GetComponent<UnitComponent>();
            var fishUnitList = unitComponent.GetFishUnitList();
            BattleLogicComponent.Instance.Foreach(fishUnitList, SetFishUnitPauseState, isSkillStart);
        }
        
        private static void SetFishUnitPauseState(Unit fishUnit, bool isPause)
        {
            FishUnitComponent fishUnitComponent = fishUnit.FishUnitComponent;
            fishUnitComponent.MoveInfo.IsPause = isPause;
        }

        /// <summary> 渔场切换场景处理 </summary>
        internal static void QuickMoveAllFish(this FisheryComponent self)
        {
            UnitComponent unitComponent = self.Parent.GetComponent<UnitComponent>();
            if (unitComponent == null)
                return;

            var fishUnitList = unitComponent.GetFishUnitList();
            ForeachHelper.Foreach(fishUnitList, QuickMove);
        }

        private static void QuickMove(Unit fishUnit)
        {
            FishUnitComponent fishUnitComponent = fishUnit.FishUnitComponent;
            fishUnitComponent.ResumeMove();
            fishUnitComponent.SetMoveSpeed(FishConfig.QuickMoveSpeed);
        }
    }
}