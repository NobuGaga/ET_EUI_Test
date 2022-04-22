// Battle Review Before Boss Node

using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 优化热更层使用 Foreach, 这里用来特殊处理 HashSet 带参数的遍历
    /// 对应遍历对象的无参数拓展在这里使用私有实现
    /// </summary>
    [FriendClass(typeof(BattleLogicComponent))]
    public static class BattleLogicComponentForeachSystem
    {
        internal static void Foreach(this BattleLogicComponent self, HashSet<Unit> hashSet,
                                     Action<Unit, bool> action, bool boolArgument)
        {
            self.Action_Unit_Bool = action;
            self.Argument_Bool = boolArgument;

            ForeachHelper.Foreach(hashSet, Action_Unit_Bool);
            
            self.Action_Unit_Bool = null;
        }

        private static void Action_Unit_Bool(Unit unit)
        {
            Scene scene = unit.DomainScene();
            BattleLogicComponent self = scene.GetBattleLogicComponent();
            self.Action_Unit_Bool(unit, self.Argument_Bool);
        }

        internal static void Foreach(this BattleLogicComponent self, HashSet<Unit> hashSet,
                                     Func<Unit, int, bool> func, int intArgument)
        {
            self.BreakFunc_Unit_Integer = func;
            self.Argument_Integer = intArgument;

            ForeachHelper.Foreach(hashSet, BreakFunc_Unit_Integer);

            self.BreakFunc_Unit_Integer = null;
        }

        private static bool BreakFunc_Unit_Integer(Unit unit)
        {
            Scene scene = unit.DomainScene();
            BattleLogicComponent self = scene.GetBattleLogicComponent();
            return self.BreakFunc_Unit_Integer(unit, self.Argument_Integer);
        }

        public static void Foreach(this BattleLogicComponent self, HashSet<Unit> hashSet,
                                   Func<Unit, Unit, BattleLogicComponent, bool> func,
                                   Unit unitArgument)
        {
            self.BreakFunc_Unit_Unit_Logic = func;
            self.Argument_Unit = unitArgument;

            ForeachHelper.Foreach(hashSet, BreakFunc_Unit_Unit_Logic);

            self.BreakFunc_Unit_Unit_Logic = null;
        }

        private static bool BreakFunc_Unit_Unit_Logic(Unit unit)
        {
            var self = BattleLogicComponent.Instance;
            return self.BreakFunc_Unit_Unit_Logic(unit, self.Argument_Unit, self);
        }
    }
}