using UnityEngine;

namespace ET
{
    public static class BattleUnitFactory
    {
        public static Unit CreatePlayer(Scene currentScene, UnitInfo unitInfo)
        {
            
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit          unit          = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unit.UnitType = UnitType.Player;
            unitComponent.Add( unit );
            var unitPlayerComponent = unit.AddComponent<UnitPlayerComponent>();

            unitPlayerComponent.name = unitInfo.name;

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            for (int i = 0; i < unitInfo.Ks.Count; ++i)
            {
                numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i]);
            }

            int cannonId = numericComponent.GetAsInt( NumericType.CannonId );
            unit.AddComponent<CannonComponent, int>(cannonId);

            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { CurrentScene = currentScene, Unit = unit });
            return unit;
        }
    }
}