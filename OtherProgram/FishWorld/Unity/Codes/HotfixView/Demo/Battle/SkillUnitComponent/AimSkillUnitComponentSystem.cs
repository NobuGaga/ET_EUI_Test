namespace ET
{
	public static class AimSkillUnitComponentSystem
	{
        public static void UpdateAimSkill(this SkillUnit self)
        {
            var playerSkillLogicComponent = self.Parent as PlayerSkillComponent;
            Unit playerUnit = playerSkillLogicComponent.Parent as Unit;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(self.DomainScene());
            
            // 不是自己的瞄准技能实例化预设的时候已经设置了移除点
            if (playerUnit.Id == selfPlayerUnit.Id)
                self.UpdatePosition();
        }
    }
}