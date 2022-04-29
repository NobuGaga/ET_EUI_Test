using ET.EventType;

namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(Unit))]
    public sealed class SkillComponentAwakeSystem : AwakeSystem<SkillComponent>
    {
        public override void Awake(SkillComponent self)
        {
            self.IceEndTime = 0;
            self.FixedUpdateBeforeFish = (Unit playerUnit) =>
                                              playerUnit.PlayerSkillComponent.FixedUpdateBeforeFish();
        }
    }

    [ObjectSystem]
    public sealed class SkillComponentDestroySystem : DestroySystem<SkillComponent>
    {
        public override void Destroy(SkillComponent self)
        {
            self.FixedUpdateBeforeFish = null;
            self.UpdateBeforeBullet = null;
            self.SetFishAnimatorState = null;
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(SkillComponent))]
    [FriendClass(typeof(PlayerSkillComponent))]
    public static class SkillLogicComponentSystem
    {
        public static void UseSkill(this BattleLogicComponent self, int skillType)
        {
            // 优先使用当前玩家的追踪目标
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.CurrentScene);
            if (selfPlayerUnit != null && !selfPlayerUnit.IsDisposed)
            {
                var playerSkillComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();
                long trackFishUnitId = playerSkillComponent.TrackFishUnitId;
                if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId)
                {
                    self.UseSkill(skillType, trackFishUnitId);
                    return;
                }
            }

            // 当前玩家没有目标则重新寻找目标
            UnitComponent unitComponent = self.UnitComponent;
            if (unitComponent == null)
            {
                self.UseSkill(skillType, ConstHelper.DefaultTrackFishUnitId);
                return;
            }

            Unit unit = unitComponent.GetMaxScoreFishUnit();
            self.UseSkill(skillType, unit != null ? unit.Id : ConstHelper.DefaultTrackFishUnitId);
        }

        private static void UseSkill(this BattleLogicComponent self, int skillType, long trackFishUnitId)
        {
            Scene currentScene = self.CurrentScene;
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
            var unitComponent = BattleLogicComponent.Instance.UnitComponent;
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

        public static void FixedUpdateBeforeFish()
        {
            var battleLogicComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleLogicComponent.CurrentScene;
            SkillComponent self = battleLogicComponent.SkillComponent;
            if (self.IceEndTime > 0 && TimeHelper.ServerNow() >= self.IceEndTime)
            {
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

            var playerUnitList = battleLogicComponent.UnitComponent.GetPlayerUnitList();
            if (playerUnitList != null)
                ForeachHelper.Foreach(playerUnitList, self.FixedUpdateBeforeFish);
        }

        /// <summary> 是否由技能控制自己的玩家射击 </summary>
        public static bool IsControlSelfShoot(this SkillComponent self)
        {
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);
            var playerSkillComponent = selfPlayerUnit.GetComponent<PlayerSkillComponent>();
            if (playerSkillComponent.IsAutoShoot)
                return true;

            if (playerSkillComponent.IsInSkill(SkillType.Aim))
                return true;

            if (playerSkillComponent.IsInSkill(SkillType.Laser))
                return true;

            return false;
        }
    }
}