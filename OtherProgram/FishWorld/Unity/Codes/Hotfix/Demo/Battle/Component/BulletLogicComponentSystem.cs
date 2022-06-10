// Battle Review Before Boss Node

using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public sealed class BulletLogicComponentAwakeSystem : AwakeSystem<BulletLogicComponent>
    {
        public override void Awake(BulletLogicComponent self)
        {
            // 重置子弹 Unit ID 计数器
            self.BulletId = 0;
            self.LastShootBulletTime = 0;
            self.ShootBulletCount = 0;
            self.UnvalidBulletId = 0;

            self.UnitInfo = new UnitInfo();

            // 重置单次使用子弹 Unit ID 计数器
            self.OneHitBulletId = 0;
            self.OneHitBulletIdStack = new Stack<long>(FisheryConfig.FisheryMaxBulletCount);
        }
    }

    [ObjectSystem]
    public sealed class BulletLogicComponentDestroySystem : DestroySystem<BulletLogicComponent>
    {
        public override void Destroy(BulletLogicComponent self)
        {
            self.UnitInfo = null;

            self.OneHitBulletIdStack.Clear();
            self.OneHitBulletIdStack = null;
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BulletLogicComponent))]
    [FriendClass(typeof(Unit))]
    public static class BulletLogicComponentSystem
    {
        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId,
                                           long trackFishUnitId) =>
                               self.PopUnitInfo(seatId, BulletConfig.DefaultBulletUnitId, trackFishUnitId);

        public static UnitInfo PopUnitInfo(this BulletLogicComponent self, int seatId,
                                                long bulletUnitId, long trackFishUnitId)
        {
            self.UnitInfo.Dispose();
            self.UnitInfo.InitBulletInfo(seatId, bulletUnitId, trackFishUnitId);
            return self.UnitInfo;
        }

        /// <summary> 生成自己发射的子弹 Unit ID </summary>
        private static long GenerateBulletId(this BulletLogicComponent self)
        {
            var battleLogicComponent =  BattleLogicComponent.Instance;
            var fisheryComponent = battleLogicComponent.FisheryComponent;
            int seatId = fisheryComponent.GetSelfSeatId();

            // 这里采用位置 ID 乘以每个人发射子弹上限的数字位数加多一位
            // 例如发射子弹上限为 30, 则 UnitId = seatId * 100 + BulletId;
            int bulletIdFix = BulletConfig.BulletIdFix * seatId;

            // Battle TODO delete
            ushort circleTime = 0;
            long unitId = 0;

            do
            {
                // Battle TODO delete 防止死循环异常处理
                if (circleTime++ > BulletConfig.ShootMaxBulletCount)
                {
                    Log.Error("生成自己发射的子弹 ID 逻辑异常, 超过执行循环次数");
                    return unitId;
                }

                unitId = bulletIdFix + (++self.BulletId);

                if (self.BulletId > BulletConfig.ShootMaxBulletCount)
                    self.BulletId = 0;
            }
            while (self.GetChild<Unit>(unitId) != null);

            return unitId;
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
            var self = battleLogicComponent.BulletLogicComponent;

            // 自己发射子弹使用客户端生成子弹 ID 规则
            if (unitInfo.UnitId == BulletConfig.DefaultBulletUnitId)
            {
                long unitId = self.GenerateBulletId();
                unitInfo.UnitId = unitId;
                self.ShootBulletCount++;
                self.LastShootBulletTime = TimeHelper.ServerNow();
            }
            // 其他玩家发射的子弹 ID, 如果玩家离场后再进入同一个渔场
            // 离场玩家的子弹还在渔场中, 而离场的玩家子弹 ID 被重置了
            // 再次发射子弹时, 接收到的子弹 ID 有可能跟在场的子弹 ID 重复
            else if (self.GetChild<Unit>(unitInfo.UnitId) != null)
            {
                long oldId = unitInfo.UnitId;
                // 将原来的 ID 引用移到另一个 ID 区间, 修正值为 BulletConfig.BulletIdFix * 100
                // 确保不会重复, 使用一个新的无效子弹 ID 计数器
                // 当计数器超过一个区间, 则重置
                var bulletIdFix = BulletConfig.BulletIdFix * 100;
                long newId = self.UnvalidBulletId + bulletIdFix;

                if (self.UnvalidBulletId > bulletIdFix)
                    self.UnvalidBulletId = 0;

                self.UnvalidBulletId++;

                self.Replace(oldId, newId);
                UnitMonoComponent.Instance.ReplaceBulletUnit(oldId, newId);
            }

            // 保持所有的战斗 Unit 都 Add 到 Current Scene 上, 因为 Unit 只是数据
            Unit unit = self.AddChildWithId<Unit, UnitInfo, CannonShootInfo>(unitInfo.UnitId, unitInfo,
                                                                             cannonShootInfo, true);

            var publishData = AfterUnitCreate.Instance;
            publishData.CurrentScene = currentScene;
            publishData.Unit = unit;

            Game.EventSystem.PublishClass(publishData);
        }

        public static void RemoveUnit(this BulletLogicComponent self, long unitId)
        {
            bool isSelfBullet = false;
            var unit = self.GetChild<Unit>(unitId);
            if (unit != null)
            {
                var battleLogicComponent = BattleLogicComponent.Instance;
                var fisheryComponent = battleLogicComponent.FisheryComponent;
                int selfSeatId = fisheryComponent.GetSelfSeatId();
                int seatId = unit.AttributeComponent.GetAsInt(NumericType.Pos);
                isSelfBullet = seatId == selfSeatId;
            }

            var publishData = RemoveBulletUnit.Instance;
            publishData.Set(BattleLogicComponent.Instance.CurrentScene, unitId);
            Game.EventSystem.PublishClass(publishData);

            if (isSelfBullet)
                self.ShootBulletCount--;

            UnitMonoComponent.Instance.Remove(unitId);
            self.RemoveChild(unitId);
        }

        /// <summary> 获取单次使用子弹 ID 校正值 </summary>
        public static long GetOneHitBulletIdFix()
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            var fisheryComponent = battleLogicComponent.FisheryComponent;
            int seatId = fisheryComponent.GetSelfSeatId();

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
            long bulletUnitId;
            if (self.OneHitBulletIdStack.Count > 0)
                bulletUnitId = self.OneHitBulletIdStack.Pop();
            else
                bulletUnitId = GetOneHitBulletIdFix() + (++self.OneHitBulletId);

            self.LastShootBulletTime = TimeHelper.ServerNow();

            battleLogicComponent.C2M_Fire(bulletUnitId, screenPosX, screenPosY, cannonStack, trackFishUnitId);
            battleLogicComponent.C2M_Hit(screenPosX, screenPosY, bulletUnitId, trackFishUnitId);
        }
    }
}