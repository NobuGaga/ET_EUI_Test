namespace ET
{
	public partial class C2M_SkillUse
	{
		public void Set(int skillId, long targetId)
		{
			SkillId = skillId;

			if (TargetId == null)
				TargetId = new System.Collections.Generic.List<long>();

			TargetId.Clear();

			if (targetId == BulletConfig.DefaultTrackFishUnitId)
				return;

			TargetId.Add(targetId);
		}
	}

	public partial class M2C_SkillUse
	{
		public int SkillType => SkillId;
	}
}