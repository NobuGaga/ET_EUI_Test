// Battle Review Before Boss Node

namespace ET
{
    public static class FisheryViewComponentSystem
    {
        /// <summary> 渔场冰冻技能视图处理 </summary>
        public static void FisheryIceSkill(this BattleViewComponent self)
        {
            Scene currentScene = self.CurrentScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            ForeachHelper.Foreach(unitComponent.GetFishUnitList(), SetAnimatorState);
        }

        private static void SetAnimatorState(Unit fishUnit)
        {
            AnimatorComponent animatorComponent = fishUnit.GetComponent<AnimatorComponent>();
            FishUnitComponent fishUnitComponent = fishUnit.GetComponent<FishUnitComponent>();
            if (fishUnitComponent.Info.IsPause)
                animatorComponent.PauseAnimator();
            else
                animatorComponent.RunAnimator();
        }
    }
}