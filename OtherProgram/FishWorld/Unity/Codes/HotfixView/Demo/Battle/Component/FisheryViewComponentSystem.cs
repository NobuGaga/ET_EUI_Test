using System.Collections.Generic;

namespace ET
{
    public static class FisheryViewComponentSystem
    {
        /// <summary> 渔场冰冻技能视图处理 </summary>
        public static void FisheryIceSkill(this BattleViewComponent self)
        {
            Scene currentScene = self.CurrentScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            HashSet<Unit> fishUnitList = unitComponent.GetFishUnitList();
            foreach (Unit fishUnit in fishUnitList)
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
}