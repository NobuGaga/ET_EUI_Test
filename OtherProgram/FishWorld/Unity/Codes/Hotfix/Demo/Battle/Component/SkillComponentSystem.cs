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

    public static class SkillComponentSystem
    {
        public static void UseSkill(this BattleLogicComponent self, int skillId)
        {
            if (skillId == SkillType.Ice)
            {
                self.UseSkill(skillId, BulletConfig.DefaultTrackFishUnitId);
                return;
            }

            UnitComponent unitComponent = self.GetUnitComponent();
            if (unitComponent == null)
            {
                self.UseSkill(skillId, BulletConfig.DefaultTrackFishUnitId);
                return;
            }

            Unit maxScoreUnit = unitComponent.GetMaxScoreFish();
            if (maxScoreUnit != null)
                self.UseSkill(skillId, maxScoreUnit.Id);

            self.UseSkill(skillId, BulletConfig.DefaultTrackFishUnitId);
        }

        private static void UseSkill(this BattleLogicComponent self, int skillId, long targetId)
        {
            Scene currentScene = self.CurrentScene();
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            PlayerSkillComponent skillUnitComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();

            if (!skillUnitComponent.IsUsedSkill(skillId))
            {
                self.C2M_SkillUse(skillId, targetId);
                return;
            }

            SkillUnit skillUnit = skillUnitComponent.Get(skillId);

            if (skillUnit.IsCdEnd())
                self.C2M_SkillUse(skillId, targetId);
            else
                // Battle TODO 后面改文本提示
                Log.Error("技能冷却中");
        }

        public static void UpdateSkill(this SkillComponent self, long playerUnitId, int skillType,
                                                                         int skillTime, int skillCdTime)
        {
            Scene currentScene = self.Parent as Scene;
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit playerUnit = unitComponent.Get(playerUnitId);

            // 玩家退出渔场会为 null
            if (playerUnit != null && !playerUnit.IsDisposed)
            {
                // 先更新指定数据, 再更新管理器数据
                PlayerSkillComponent skillUnitComponent = playerUnit.GetComponent<PlayerSkillComponent>();
                skillUnitComponent.Set(skillType, skillTime, skillCdTime);
            }

            // 再更新渔场技能数据
            if (skillType == SkillType.Ice)
                self.IceEndTime = skillTime + TimeHelper.ServerNow();
        }

        public static void FixedUpdate(this SkillComponent self)
        {
            if (self.IsSkillEnd())
                self.SkillEnd();

            HashSet<Unit> playerUnitList = self.GetPlayerUnitList();
            if (playerUnitList == null)
                return;

            // 先更新技能数据(PlayerSkillLogicComponent 制作结束事件派发)
            foreach (Unit playerUnit in playerUnitList)
                playerUnit.GetComponent<PlayerSkillComponent>().FixedUpdate();
        }

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

            BattleLogicComponent battleLogicComponent = currentScene.GetBattleLogicComponent();
            battleLogicComponent.FisheryIceSkill(false);

            Game.EventSystem.Publish(new FisherySkillEnd
            {
                CurrentScene = currentScene,
                SkillType = SkillType.Ice,
            });
        }
    }
}