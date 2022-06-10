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
            long playerUnitId = 0;
            // 数据已经没有了, 但视图资源还在, 先移除子弹资源
            if (bulletUnit == null)
            {
                Log.Error($"BattleMonoComponent.Collide bullet unit id not exist. bulletUnitId = { bulletUnitId }");
                UnitMonoComponent.Instance.RemoveUnvalidBulletUnit(bulletUnitId);
                PublishEventCollide(currentScene, screenPosX, screenPosY, playerUnitId, fishUnitId);
                return;
            }

            var attributeComponent = bulletUnit.GetComponent<NumericComponent>();
            int seatId = attributeComponent.GetAsInt(NumericType.Pos);
            var fisheryComponent = self.FisheryComponent;
            // Battle Warning 有玩家退出房间 Current Scene 上的 FisheryComponent 组件会为空
            // 暂时将 playerUnitId 设置为 0
            if (fisheryComponent != null)
            {
                Unit playerUnit = fisheryComponent.GetPlayerUnit(seatId);
                // Battle Warning 这个也会为空
                if (playerUnit != null)
                    playerUnitId = playerUnit.Id;
            }

            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);

            if (playerUnitId == selfPlayerUnit.Id)
                self.C2M_Hit(screenPosX, screenPosY, bulletUnitId, fishUnitId);

            bulletLogicComponent.RemoveUnit(bulletUnitId);
            PublishEventCollide(currentScene, screenPosX, screenPosY, playerUnitId, fishUnitId);
        }

        private static void PublishEventCollide(Scene currentScene, float screenPosX, float screenPosY, long playerUnitId, long fishUnitId)
        {
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