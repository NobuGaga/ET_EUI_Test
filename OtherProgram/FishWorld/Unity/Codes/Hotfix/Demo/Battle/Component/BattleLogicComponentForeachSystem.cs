// Battle Review Before Boss Node

using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 优化热更层使用 Foreach, 这里用来特殊处理 HashSet 带参数的遍历
    /// 对应遍历对象的无参数拓展在这里使用私有实现
    /// </summary>
    internal static class BattleLogicComponentForeachSystem
    {
        internal static void Foreach(this BattleLogicComponent self, HashSet<Unit> hashSet,
                                   Action<Unit, bool> action, bool boolArgument)
        {
            self.Action_Unit_Bool = action;
            self.Argument_Bool = boolArgument;

            ForeachHelper.Foreach(hashSet, Action_Unit_Bool);
            
            self.Action_Unit_Bool = null;
        }

        private static void Action_Unit_Bool(this Unit unit)
        {
            Scene scene = unit.DomainScene();
            BattleLogicComponent self = scene.GetBattleLogicComponent();
            self.Action_Unit_Bool(unit, self.Argument_Bool);
        }

        internal static void Foreach(this BattleLogicComponent self, HashSet<Unit> hashSet,
                                   Action<Unit, int> action, int integerArgument)
        {
            self.Action_Unit_Integer = action;
            self.Argument_Integer = integerArgument;

            ForeachHelper.Foreach(hashSet, Action_Unit_Integer);

            self.Action_Unit_Integer = null;
        }

        private static void Action_Unit_Integer(this Unit unit)
        {
            Scene scene = unit.DomainScene();
            BattleLogicComponent self = scene.GetBattleLogicComponent();
            self.Action_Unit_Integer(unit, self.Argument_Integer);
        }
    }
}