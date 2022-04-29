using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public sealed class BulletLogicComponentAwakeSystem : AwakeSystem<BulletLogicComponent>
    {
        public override void Awake(BulletLogicComponent self) => self.Reset();
    }

    [ObjectSystem]
    public sealed class BulletLogicComponentDestroySystem : DestroySystem<BulletLogicComponent>
    {
        public override void Destroy(BulletLogicComponent self) => self.RemoveAllUnit();
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BulletLogicComponent))]
    [FriendClass(typeof(Unit))]
    public static class BulletLogicComponentSystem
    {
        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId)
        {
            self.unitInfo.Dispose();
            self.unitInfo.InitBulletInfo(seatId);
            return self.unitInfo;
        }

        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId,
                                           long trackFishUnitId)
        {
            self.unitInfo.Dispose();
            self.unitInfo.InitBulletInfo(seatId, BulletConfig.DefaultBulletUnitId, trackFishUnitId);
            return self.unitInfo;
        }

        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId,
                                                long bulletUnitId, long trackFishUnitId)
        {
            self.unitInfo.Dispose();
            self.unitInfo.InitBulletInfo(seatId, bulletUnitId, trackFishUnitId);
            return self.unitInfo;
        }

        /// <summary> 生成自己发射的子弹 Unit ID </summary>
        private static long GenerateBulletId(this BulletLogicComponent self)
        {
            long unitId = 0;

            Scene currentScene = self.Parent as Scene;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            NumericComponent numericComponent = selfPlayerUnit.GetComponent<NumericComponent>();
            int seatId = numericComponent.GetAsInt(NumericType.Pos);

            // 这里采用位置 ID 乘以每个人发射子弹上限的数字位数加多一位
            // 例如发射子弹上限为 30, 则 UnitId = seatId * 100 + BulletId;
            int bulletIdFix = BulletConfig.BulletIdFix * seatId;

            // Battle TODO delete
            ushort circleTime = 0;

            do
            {
                // Battle TODO delete 防止死循环异常处理
                if (circleTime++ > BulletConfig.ShootMaxBulletCount)
                {
                    Log.Error("生成自己发射的子弹 ID 逻辑异常, 超过执行循环次数");
                    return unitId;
                }

                unitId = bulletIdFix + (++self.BulletId);

                if (self.BulletId >= BulletConfig.ShootMaxBulletCount)
                    self.BulletId = 0;
            }
            while (self.GetChild<Unit>(unitId) != null);

            return unitId;
        }

        /// <summary> 生成马上销毁的子弹 Unit ID </summary>
        private static long GenerateOneHitBulletId(this BulletLogicComponent self)
        {
            if (self.OneHitBulletIdStack.Count > 0)
                return self.OneHitBulletIdStack.Pop();

            return self.GetOneHitBulletIdFix() + self.OneHitBulletId++;
        }

        /// <summary> 获取单次使用子弹 ID 校正值 </summary>
        public static long GetOneHitBulletIdFix(this BulletLogicComponent self)
        {
            Scene currentScene = self.Parent as Scene;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            NumericComponent numericComponent = selfPlayerUnit.GetComponent<NumericComponent>();
            int seatId = numericComponent.GetAsInt(NumericType.Pos);

            // 这里采用位置 ID 乘以每个人发射子弹上限的数字位数加多两位
            // 例如发射子弹上限为 30, 则 UnitId = seatId * 1000 + OneHitBulletId;
            return BulletConfig.BulletIdFix * seatId * 10;
        }

        public static ushort CheckSelfSkillShootState(this BattleLogicComponent battleLogicComponent)
        {
            var self = battleLogicComponent.BulletLogicComponent;
            var shootInterval = TimeHelper.ServerNow() - self.LastShootBulletTime;
            if (shootInterval < BulletConfig.ShootBulletInterval)
                return BattleCodeConfig.ShootIntervalLimit;

            if (self.ShootBulletCount >= BulletConfig.ShootMaxBulletCount)
                return BattleCodeConfig.UpperLimitBullet;

            Unit playerUnit = UnitHelper.GetMyUnitFromZoneScene(battleLogicComponent.ZoneScene);
            var attributeComponent = playerUnit.GetComponent<NumericComponent>();
            int coin = attributeComponent.GetAsInt(NumericType.Coin);
            int cannonStack = attributeComponent.GetAsInt(NumericType.CannonStack);
            if (coin < cannonStack)
                return BattleCodeConfig.NotEnoughMoney;

            return BattleCodeConfig.Success;
        }

        /// <summary> 战斗逻辑发射协议子弹, 直接创建子弹 ID 然后再发送击中协议过去 </summary>
        public static void Shoot_C2M_Bullet(this BattleLogicComponent battleLogicComponent, float screenPosX,
                                            float screenPosY, int cannonStack, long trackFishUnitId)
        {
            var self = battleLogicComponent.BulletLogicComponent;

            // 生成一个不跟普通子弹重复的 ID
            // 然后只做时间间隔的刷新, 不加入子弹碰撞列表中
            // 因为发射出去后马上当成已经发生碰撞的子弹
            long bulletUnitId = self.GenerateOneHitBulletId();

            self.LastShootBulletTime = TimeHelper.ServerNow();

            battleLogicComponent.C2M_Fire(bulletUnitId, screenPosX, screenPosY, cannonStack, trackFishUnitId);
            battleLogicComponent.C2M_Hit(screenPosX, screenPosY, bulletUnitId, trackFishUnitId);
        }

        /// <summary> 
        /// 战斗逻辑发射子弹, 放在子弹这里定义为了使用这里的子弹所特有的方法
        /// Battle Warning 必要在调用前调用 UIFisheriesComponent.CalcCannonRotation
        /// 修改炮台旋转方向为当前设计方向
        /// </summary>
        public static void ShootBullet(this BattleLogicComponent battleLogicComponent,
                                            UnitInfo unitInfo, CannonShootInfo cannonShootInfo)
        {
            Scene currentScene = battleLogicComponent.CurrentScene;
            BulletLogicComponent self = battleLogicComponent.BulletLogicComponent;

            if (unitInfo.UnitId == BulletConfig.DefaultBulletUnitId)
            {
                long unitId = self.GenerateBulletId();
                unitInfo.UnitId = unitId;
            }

            bool isUseModelPool = BattleConfig.IsUseModelPool;

            // 保持所有的战斗 Unit 都 Add 到 Current Scene 上, 因为 Unit 只是数据
            Unit unit = self.AddChildWithId<Unit, UnitInfo, CannonShootInfo>(unitInfo.UnitId, unitInfo,
                                                                             cannonShootInfo, isUseModelPool);

            self.ShootBulletCount++;
            self.BulletIdList.Add(unitInfo.UnitId);

            self.LastShootBulletTime = TimeHelper.ServerNow();

            var publishData = AfterUnitCreate.Instance;
            publishData.CurrentScene = currentScene;
            publishData.Unit = unit;
            Game.EventSystem.PublishClass(publishData);
        }

        public static void RemoveUnit(this BulletLogicComponent self, long unitId)
        {
            var publishData = RemoveBulletUnit.Instance;
            publishData.CurrentScene = BattleLogicComponent.Instance.CurrentScene;
            publishData.UnitId = unitId;
            Game.EventSystem.PublishClass(publishData);

            UnitMonoComponent.Instance.Remove(unitId);

            self.ShootBulletCount--;
            
            for (short index = (short)(self.BulletIdList.Count - 1); index >= 0; index--)
            {
                if (self.BulletIdList[index] == unitId)
                {
                    self.BulletIdList.RemoveAt(index);
                    self.RemoveChild(unitId);
                    break;
                }
            }
        }

        public static void RemoveAllUnit(this BulletLogicComponent self)
        {
            for (short index = 0; index < self.BulletIdList.Count; index++)
            {
                long unitId = self.BulletIdList[index];
                self.RemoveChild(unitId);
            }

            self.Reset();
        }

        public static void Reset(this BulletLogicComponent self)
        {
            // 重置子弹 Unit ID 计数器
            self.BulletId = 0;

            // 重置单次使用子弹 Unit ID 计数器
            self.OneHitBulletId = 0;

            // 重置自己发射子弹个数
            self.ShootBulletCount = 0;

            self.LastShootBulletTime = 0;

            // 不在 Awake 创建, 在定义的时候 new, 在 Destroy 清理, 更改是否使用池的标识时不用改写代码
            self.BulletIdList.Clear();

            // 不在 Awake 创建, 在定义的时候 new, 在 Destroy 清理, 更改是否使用池的标识时不用改写代码
            self.OneHitBulletIdStack.Clear();
        }
    }
}