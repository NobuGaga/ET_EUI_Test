using ET.EventType;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public sealed class SkillComponentAwakeSystem : AwakeSystem<SkillComponent>
    {
        public override void Awake(SkillComponent self) => self.IceEndTime = 0;
    }

    [ObjectSystem]
    public sealed class SkillComponentDestroySystem : DestroySystem<SkillComponent>
    {
        public override void Destroy(SkillComponent self)
        {
            // Battle TODO
        }
    }

    [FriendClass(typeof(SkillComponent))]
    [FriendClass(typeof(PlayerSkillComponent))]
    public static class SkillLogicComponentSystem
    {
        public static void UseSkill(this BattleLogicComponent self, int skillType)
        {
            // 优先使用当前玩家的追踪目标
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.CurrentScene());
            if (selfPlayerUnit != null && !selfPlayerUnit.IsDisposed)
            {
                var playerSkillComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();
                long trackFishUnitId = playerSkillComponent.TrackFishUnitId;
                if (trackFishUnitId != BulletConfig.DefaultTrackFishUnitId)
                {
                    self.UseSkill(skillType, trackFishUnitId);
                    return;
                }
            }

            // 当前玩家没有目标则重新寻找目标
            Scene currentScene = self.CurrentScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                self.UseSkill(skillType, BulletConfig.DefaultTrackFishUnitId);
                return;
            }

            Unit unit = unitComponent.GetMaxScoreFishUnit();
            self.UseSkill(skillType, unit != null ? unit.Id : BulletConfig.DefaultTrackFishUnitId);
        }

        private static void UseSkill(this BattleLogicComponent self, int skillType, long trackFishUnitId)
        {
            Scene currentScene = self.CurrentScene();
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            PlayerSkillComponent skillUnitComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();

            if (!skillUnitComponent.IsUsedSkill(skillType))
            {
                self.C2M_SkillUse(skillType, trackFishUnitId);
                return;
            }

            SkillUnit skillUnit = skillUnitComponent.Get(skillType);

            if (skillUnit.IsCdEnd())
                self.C2M_SkillUse(skillType, trackFishUnitId);
            else
                // Battle TODO 后面改文本提示
                Log.Error("技能冷却中");
        }

        public static void UpdateSkill(this SkillComponent self, M2C_SkillUse message)
        {
            Scene currentScene = self.Parent as Scene;
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit playerUnit = unitComponent.Get(message.UnitId);

            // 玩家退出渔场会为 null
            if (playerUnit != null && !playerUnit.IsDisposed)
            {
                // 先更新指定数据, 再更新管理器数据
                PlayerSkillComponent skillUnitComponent = playerUnit.GetComponent<PlayerSkillComponent>();
                skillUnitComponent.Set(message);
            }

            // 再更新渔场技能数据
            if (message.SkillType == SkillType.Ice)
                self.IceEndTime = message.SkillTime + TimeHelper.ServerNow();
        }

        public static void FixedUpdateBeforeFish(this SkillComponent self)
        {
            if (self.IsSkillEnd())
                self.SkillEnd();

            var playerUnitList = self.GetPlayerUnitList();
            if (playerUnitList != null)
                ForeachHelper.Foreach(playerUnitList, FixedUpdateSkillBeforeFish);
        }

        private static void FixedUpdateSkillBeforeFish(this Unit playerUnit) =>
                            playerUnit.GetComponent<PlayerSkillComponent>().FixedUpdateBeforeFish();

        public static HashSet<Unit> GetPlayerUnitList(this SkillComponent self)
        {
            Scene currentScene = self.Parent as Scene;
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            if (unitComponent == null)
                return null;

            return unitComponent.GetPlayerUnitList();
        }

        /// <summary> 技能效果是否存在 </summary>
        private static bool IsRunning(this SkillComponent self) => self.IceEndTime > 0;

        /// <summary> 是否由技能控制自己的玩家射击 </summary>
        public static bool IsControlSelfShoot(this SkillComponent self)
        {
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.DomainScene());
            var playerSkillComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();
            if (playerSkillComponent.IsAutoShoot)
                return true;

            if (playerSkillComponent.IsInSkill(SkillType.Aim))
                return true;

            if (playerSkillComponent.IsInSkill(SkillType.Laser))
                return true;

            return false;
        }

        /// <summary> 技能效果是否结束 </summary>
        private static bool IsSkillEnd(this SkillComponent self) =>
                            self.IsRunning() && TimeHelper.ServerNow() >= self.IceEndTime;

        private static void SkillEnd(this SkillComponent self)
        {
            Scene currentScene = self.Parent as Scene;

            // 技能效果时间到了之后情况时间戳, 保证事件只触发一次
            // 后面逻辑通过时间戳是否大于零表示技能是否还在效果时间
            self.IceEndTime = 0;

            var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            fisheryComponent.FisheryIceSkill(false);

            Game.EventSystem.Publish(new FisherySkillEnd
            {
                CurrentScene = currentScene,
                SkillType = SkillType.Ice,
            });
        }
    }
}