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

        ///<summary> 检查自己玩家是否可以进行普通射击 </summary>
        /// <returns> 战斗编码, 用于视图层处理 </returns>
        public static ushort CheckSelfNormalShootState(this BattleLogicComponent battleLogicComponent)
        {
            // 优先级搞得在前面先判断, 后面复杂了在根据 Code 定义优先级来调用
            Scene currentScene = battleLogicComponent.CurrentScene();
            SkillComponent skillComponent = currentScene.GetComponent<SkillComponent>();
            if (skillComponent.IsControlSelfShoot())
                return BattleCodeConfig.SkillControl;

            return battleLogicComponent.CheckSelfSkillShootState();
        }

        public static ushort CheckSelfSkillShootState(this BattleLogicComponent battleLogicComponent)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            var shootInterval = TimeHelper.ServerNow() - battleLogicComponent.LastShootBulletTime;
            if (shootInterval < BulletConfig.ShootBulletInterval)
                return BattleCodeConfig.ShootIntervalLimit;

            BulletLogicComponent self = currentScene.GetComponent<BulletLogicComponent>();
            if (self.ShootBulletCount >= BulletConfig.ShootMaxBulletCount)
                return BattleCodeConfig.UpperLimitBullet;

            Unit playerUnit = UnitHelper.GetMyUnitFromZoneScene(battleLogicComponent.ZoneScene());
            var attributeComponent = playerUnit.GetComponent<NumericComponent>();
            int coin = attributeComponent.GetAsInt(NumericType.Coin);
            int cannonStack = attributeComponent.GetAsInt(NumericType.CannonStack);
            if (coin < cannonStack)
                return BattleCodeConfig.NotEnoughMoney;

            return BattleCodeConfig.Success;
        }

        /// <summary> 
        /// 战斗逻辑发射子弹, 放在子弹这里定义为了使用这里的子弹所特有的方法
        /// Battle Warning 必要在调用前调用 UIFisheriesComponent.CalcCannonRotation
        /// 修改炮台旋转方向为当前设计方向
        /// </summary>
        public static void ShootBullet(this BattleLogicComponent battleLogicComponent,
                                            UnitInfo unitInfo, CannonShootInfo cannonShootInfo)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            BulletLogicComponent self = currentScene.GetComponent<BulletLogicComponent>();

            if (unitInfo.UnitId == BulletConfig.DefaultBulletUnitId)
            {
                long unitId = self.GenerateBulletId();
                unitInfo.UnitId = unitId;
            }

            bool isUseModelPool = BattleTestConfig.IsUseModelPool;

            // 保持所有的战斗 Unit 都 Add 到 Current Scene 上, 因为 Unit 只是数据
            Unit unit = self.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId, isUseModelPool);
            // Add BattleUnitLogicComponent 前要对 UnitType 进行赋值
            unit.UnitType = unitInfo.UnitType;
            unit.AddComponent<BattleUnitLogicComponent, UnitInfo, CannonShootInfo>(unitInfo, cannonShootInfo, isUseModelPool);

            self.ShootBulletCount++;
            self.BulletIdList.Add(unitInfo.UnitId);

            battleLogicComponent.LastShootBulletTime = TimeHelper.ServerNow();

            Game.EventSystem.Publish(new AfterUnitCreate() { CurrentScene = currentScene, Unit = unit });
        }

        public static void RemoveUnit(this BulletLogicComponent self, long unitId)
        {
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

            // 重置自己发射子弹个数
            self.ShootBulletCount = 0;

            // 不在 Awake 创建, 在定义的时候 new, 在 Destroy 清理, 更改是否使用池的标识时不用改写代码
            self.BulletIdList.Clear();
        }
    }
}