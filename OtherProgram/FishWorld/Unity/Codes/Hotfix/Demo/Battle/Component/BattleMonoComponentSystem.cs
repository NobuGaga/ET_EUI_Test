namespace ET
{
    /// <summary>
    /// 跟 Mono 层交互的数据结构拓展方法在这里定义
    /// 使用 Extension 后缀跟 Model 层数据结构区分, 这是在 Mono 层定义的数据结构(类)
    /// </summary>
    [FriendClass(typeof(BattleLogicComponent))]
    public static class BattleMonoComponentSystem
    {
        /// <summary> 碰撞处理 </summary>
        public static void Collide(float screenPosX, float screenPosY, long bulletUnitId, long fishUnitId)
        {
            var self = BattleLogicComponent.Instance;
            Scene currentScene = self.CurrentScene;

            // 子弹的移除都是本地客户端判定是否碰撞来进行
            BulletLogicComponent bulletLogicComponent = self.BulletLogicComponent;
            Unit bulletUnit = bulletLogicComponent.GetChild<Unit>(bulletUnitId);
            var attributeComponent = bulletUnit.GetComponent<NumericComponent>();
            int seatId = attributeComponent.GetAsInt(NumericType.Pos);
            Unit playerUnit = FisheryHelper.GetPlayerUnit(seatId);
            long playerUnitId = playerUnit.Id;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);

            if (playerUnitId == selfPlayerUnit.Id)
                self.C2M_Hit(screenPosX, screenPosY, bulletUnitId, fishUnitId);

            bulletLogicComponent.RemoveUnit(bulletUnitId);

            var publishData = EventType.BulletCollideFish.Instance;
            publishData.CurrentScene = currentScene;
            publishData.ScreenPosX = screenPosX;
            publishData.ScreenPosY = screenPosY;
            publishData.PlayerUnitId = playerUnitId;
            publishData.FishUnitId = fishUnitId;
            Game.EventSystem.PublishClass(publishData);
        }

        public static void RemoveFishUnit(long fishUnitId)
        {
            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
            unitComponent?.Remove(fishUnitId);
        }
    }
}