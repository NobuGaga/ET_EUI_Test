// Battle Review Before Boss Node

using ET.EventType;

using BattleViewUnit = ET.UnitViewComponentSystem;

namespace ET
{
    /// <summary> 原 UnitComponent 组件视图显示拓展 </summary>
	internal static class UnitComponentViewSystem
    {
        internal static void UpdateFishUnitList(this UnitComponent self) =>
                             ForeachHelper.Foreach(self.GetFishUnitList(), BattleViewUnit.Update);
    }

    [FriendClass(typeof(Unit))]
    public class RemoveUnit_UnitComponent : AEvent<RemoveUnit>
    {
        protected override void Run(RemoveUnit args)
        {
            UnitComponent unitComponent = args.CurrentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.Get(args.UnitId);
            if (unit.Type != UnitType.Fish)
                return;

            AnimatorComponent animatorComponent = unit.GetComponent<AnimatorComponent>();
            animatorComponent.PauseAnimator();

            BattleLogic.Remove(args.UnitId);
        }
    }
}