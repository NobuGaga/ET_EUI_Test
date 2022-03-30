using ET.EventType;

namespace ET
{
    #region Event

    public class AfterCreateCurrentScene_BulletLogicComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(AfterCreateCurrentScene args)
        {
            // Bullet Unit Component 要挂在 Current Scene 上面, 释放的时候跟 Unit Component 保持一直
            args.CurrentScene.AddComponent<BulletLogicComponent>();
            await ETTask.CompletedTask;
        }
    }

    #endregion

    #region Life Circle

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

    #endregion

    #region Base Function

    public static class BulletLogicComponentSystem
    {
        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId)
        {
            self.unitInfo.Dispose();
            self.unitInfo.InitBulletInfo(seatId);
            return self.unitInfo;
        }

        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId,
                                                long bulletUnitId, long trackFishUnitId)
        {
            self.unitInfo.Dispose();
            self.unitInfo.InitBulletInfo(seatId, bulletUnitId, trackFishUnitId);
            return self.unitInfo;
        }

        /// <summary> 到达自己发射炮弹的上限 </summary>
        private static bool IsShootLimit(this BulletLogicComponent self)
        {
            bool isShootLimit = self.ShootBulletCount >= BulletConfig.ShootMaxBulletCount;
            if (isShootLimit)
                Log.Error($"发射子弹超出配置表配置上限 : { BulletConfig.ShootMaxBulletCount }");

            return isShootLimit;
        }

        /// <summary> 生成自己发射的子弹 Unit ID </summary>
        private static void TryGenerateBulletId(this BulletLogicComponent self, out long unitId)
        {
            unitId = 0;

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
                    return;
                }

                unitId = bulletIdFix + (++self.BulletId);

                if (self.BulletId >= BulletConfig.ShootMaxBulletCount)
                    self.BulletId = 0;
            }
            while (self.GetChild<Unit>(unitId) != null);
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
                if (self.IsShootLimit())
                    return;

                self.TryGenerateBulletId(out long unitId);
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

    #endregion
}