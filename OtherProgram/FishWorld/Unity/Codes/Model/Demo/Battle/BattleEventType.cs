namespace ET
{
    namespace EventType
    {
        /// <summary>
        /// 实例化战斗模型事件, 在该事件之前添加 GameObjectComponent
        /// 使用 BattleUnitComponent.GameObjectComponent() 获取
        /// </summary>
        public struct AfterBattleGameObjectCreate
        {
            public Unit Unit;
        }
    }
}