namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
	public static class AimSkillUnitComponentSystem
	{
        public static void UpdateAimSkill(this SkillUnit self)
        {
            Entity playerUnit = self.Parent.Parent;
            var battleViewComponent = BattleLogicComponent.Instance;
            Scene currentScene = battleViewComponent.CurrentScene;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);

            // 不是自己的瞄准技能实例化预设的时候已经设置了移除点
            if (playerUnit.Id == selfPlayerUnit.Id)
                self.UpdatePosition();

            BattleViewComponent.Instance.SkillShoot(playerUnit.Id, self);
        }
    }
}