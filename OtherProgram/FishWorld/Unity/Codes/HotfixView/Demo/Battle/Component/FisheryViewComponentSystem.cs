// Battle Review Before Boss Node

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(SkillComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class FisheryViewComponentSystem
    {
        /// <summary> 渔场冰冻技能视图处理 </summary>
        public static void FisheryIceSkill()
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            var unitComponent = battleLogicComponent.UnitComponent;
            var skillComponent = battleLogicComponent.SkillComponent;
            ForeachHelper.Foreach(unitComponent.GetFishUnitList(), skillComponent.SetFishAnimatorState);
        }

        public static void SetFishAnimatorState(Unit fishUnit) => fishUnit.PauseAnimation();
    }
}