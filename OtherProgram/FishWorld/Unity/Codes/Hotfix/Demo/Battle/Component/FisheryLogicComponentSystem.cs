using System.Collections.Generic;

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

    [ObjectSystem]
    public sealed class FisheryComponentDestroySystem : DestroySystem<FisheryComponent>
    {
        public override void Destroy(FisheryComponent self)
        {
            // Battle TODO
        }
    }

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
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Parent as Scene);
            return selfPlayerUnit.GetSeatId();
        }

        /// <summary> 渔场冰冻技能逻辑处理 </summary>
        /// <param name="isSkillStart">是否开始使用冰冻技能</param>
        public static void FisheryIceSkill(this BattleLogicComponent self, bool isSkillStart)
        {
            Scene currentScene = self.CurrentScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            HashSet<Unit> fishUnitList = unitComponent.GetFishUnitList();
            foreach (Unit fishUnit in fishUnitList)
            {
                FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
                fishUnitComponent.Info.IsPause = isSkillStart;
            }
        }

        /// <summary> 渔场切换场景处理 </summary>
        public static void QuickMoveFish(this BattleLogicComponent self)
        {
            UnitComponent unitComponent = self.GetUnitComponent();
            if (unitComponent == null)
                return;

            HashSet<Unit> fishUnitList = unitComponent.GetFishUnitList();
            foreach (Unit fishUnit in fishUnitList)
            {
                FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
                fishUnitComponent.ResumeMove();
                fishUnitComponent.SetMoveSpeed(FishConfig.QuickMoveSpeed);
            }
        }
    }
}