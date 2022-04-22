// Battle Review Before Boss Node

using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    /// <summary> 原 UnitComponent 组件视图显示拓展 </summary>
    [FriendClass(typeof(BattleLogicComponent))]
	internal static class UnitComponentViewSystem
    {
        internal static void UpdateFishUnitList(this UnitComponent self, BattleLogicComponent battleLogicComponent)
        {
            List<long> fishUnitIdList = battleLogicComponent.FishUnitIdList;
            for (int index = fishUnitIdList.Count - 1; index >= 0; index--)
            {
                Unit fishUnit = self.GetChild<Unit>(fishUnitIdList[index]);
                fishUnit.Update();
            }
        }
    }

    [FriendClass(typeof(Unit))]
    public class RemoveUnit_UnitComponent : AEventClass<RemoveUnit>
    {
        protected override void Run(object obj)
        {
            var args = obj as RemoveUnit;
            UnitComponent unitComponent = args.CurrentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.Get(args.UnitId);
            if (unit.Type != UnitType.Fish)
                return;

            AnimatorComponent animatorComponent = unit.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
                animatorComponent.PauseAnimator();

            UnitMonoComponent.Remove(args.UnitId);
        }
    }
}