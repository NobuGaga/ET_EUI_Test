namespace ET
{
	public static class BattleCodeConfig
    {
        public const ushort Success = 0;

        public const ushort Error = 1;

        /// <summary> 超出发射子弹上限 </summary>
        public const ushort UpperLimitBullet = 2;

        /// <summary> 金币不足 </summary>
        public const ushort NotEnoughMoney = 3;

        /// <summary> 射击间隔限制 </summary>
        public const ushort ShootIntervalLimit = 4;

        /// <summary> 技能控制射击 </summary>
        public const ushort SkillControl = 5;
    }
}