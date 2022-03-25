using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    #region Life Circle

    [ObjectSystem]
    public sealed class BulletLogicComponentAwakeSystem : AwakeSystem<BulletLogicComponent>
    {
        public override void Awake(BulletLogicComponent self)
        {
            // 重置子弹 Unit ID 计数器
            self.BulletId = 0;

            // 重置自己发射子弹个数
            self.ShootBulletCount = 0;
        }
    }

    [ObjectSystem]
    public sealed class BulletLogicComponentDestroySystem : DestroySystem<BulletLogicComponent>
    {
        public override void Destroy(BulletLogicComponent self)
        {
            // 不在 Awake 创建, 在定义的时候 new, 在 Destroy 清理, 更改是否使用池的标识时不用改写代码
            self.BulletIdList.Clear();
            self.BulletUnitMap.Clear();
        }
    }

    #endregion

    #region Base Function

    public static class BulletLogicComponentSystem
    {
        /// <summary> 到达自己发射炮弹的上限 </summary>
        private static bool IsShootLimit(this BulletLogicComponent self) => 
                                                    self.ShootBulletCount >= BulletConfig.ShootMaxBulletCount;

        /// <summary> 生成自己发射的子弹 Unit ID </summary>
        private static bool TryGenerateBulletId(this BulletLogicComponent self, out long unitId)
        {
            unitId = 0;

            if (self.IsShootLimit())
                return false;

            Scene currentScene = self.Parent as Scene;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            NumericComponent numericComponent = selfPlayerUnit.GetComponent<NumericComponent>();
            int seatId = numericComponent.GetAsInt(NumericType.Pos);

            Dictionary<long, Unit> bulletUnitMap = self.BulletUnitMap;

            // 这里采用位置 ID 乘以每个人发射子弹上限的数字位数加多一位
            // 例如发射子弹上限为 30, 则 UnitId = seatId * 100 + BulletId;
            int bulletIdFix = BulletConfig.BulletIdFix * seatId;

            // Battle TODO delete
            ushort circleTime = 0;

            do
            {
                // Battle TODO delete 防止死循环异常处理
                if (circleTime++ > BulletConfig.ShootMaxBulletCount)
                    throw new Exception("生成自己发射的子弹 ID 逻辑异常, 超过执行循环次数");

                unitId = bulletIdFix + (++self.BulletId);

                if (self.BulletId >= BulletConfig.ShootMaxBulletCount)
                    self.BulletId = 0;
            }
            while (bulletUnitMap.ContainsKey(unitId));

            return true;
        }

        /// <summary> 
        /// 战斗逻辑发射子弹, 放在子弹这里定义为了使用这里的子弹所特有的方法
        /// Battle Warning 必要在调用前调用 UIFisheriesComponent.CalcCannonRotation
        /// 修改炮台旋转方向为当前设计方向
        /// </summary>
        public static void ShootBullet(this BattleLogicComponent battleLogicComponent, CannonShootInfo info)
        {
            Scene currentScene = battleLogicComponent.CurrentScene();
            FisheryComponent fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            int seatId = fisheryComponent.GetSelfSeatId();
            BulletLogicComponent self = currentScene.GetComponent<BulletLogicComponent>();
            try
            {
                if (self.TryGenerateBulletId(out long unitId))
                    // Battle TODO 暂时写死追踪鱼 ID
                    self.AddUnit(seatId, unitId, 0);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static void AddUnit(this BulletLogicComponent self, long playerId, long bulletUnitId, long trackFishUnitId)
        {
            Scene currentScene = self.DomainScene();
            // Player Unit 在 UnitComponent 上储存
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            int seatId = unitComponent.GetSeatId(playerId);
            self.AddUnit(seatId, bulletUnitId, trackFishUnitId);
        }

        private static void AddUnit(this BulletLogicComponent self, int seatId, long bulletUnitId, long trackFishUnitId)
        {
            Scene currentScene = self.DomainScene();
            bool isUseModelPool = BattleTestConfig.IsUseModelPool;
            int bulletUnitConfigId = BulletConfig.BulletUnitConfigId;

            // 保持所有的战斗 Unit 都 Add 到 Current Scene 上, 因为 Unit 只是数据
            Unit unit = currentScene.AddChildWithId<Unit, int>(bulletUnitId, bulletUnitConfigId, isUseModelPool);
            unit.InitBulletUnit(seatId, trackFishUnitId);

            self.ShootBulletCount++;
            self.BulletIdList.Add(bulletUnitId);
            self.BulletUnitMap.Add(bulletUnitId, unit);

            Game.EventSystem.Publish(new EventType.AfterUnitCreate() { CurrentScene = currentScene, Unit = unit });
        }

        /// <summary>
        /// 这里不修改 Unit 的 Awake 参数, 使用拓展 Unit 方法实现
        /// 初始化子弹 Unit 处理
        /// </summary>
        private static void InitBulletUnit(this Unit self, int seatId, long trackFishUnitId)
        {
            self.UnitType = UnitType.Bullet;

            UnitInfo unitInfo = new UnitInfo()
            {
                Ks = new List<int>() { NumericType.Pos, NumericType.TrackFishId },
                Vs = new List<long>() { seatId, trackFishUnitId },
            };
            
            self.AddComponent<BattleUnitLogicComponent, UnitInfo>(unitInfo);
        }

        public static void RemoveUnit(this BulletLogicComponent self, long unitId)
        {
            self.ShootBulletCount--;
            
            for (short index = (short)(self.BulletIdList.Count - 1); index >= 0; index--)
            {
                if (self.BulletIdList[index] == unitId)
                {
                    self.BulletIdList.Add(unitId);
                    break;
                }
            }

            self.BulletUnitMap.Remove(unitId);
        }
    }

    #endregion
}