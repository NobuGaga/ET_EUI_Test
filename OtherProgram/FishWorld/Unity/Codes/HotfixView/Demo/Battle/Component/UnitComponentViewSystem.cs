using System.Collections.Generic;

using BattleViewUnit = ET.UnitViewComponentSystem;

namespace ET
{
    /// <summary> 原 UnitComponent 组件视图显示拓展 </summary>
	internal static class UnitComponentViewSystem
    {
        internal static void UpdateFishUnitList(this UnitComponent self)
        {
            HashSet<Unit> fishUnitList = self.GetFishUnitList();
            ForeachHelper.Foreach(fishUnitList, BattleViewUnit.Update);
        }
    }
}