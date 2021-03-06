namespace ET
{
    /// <summary>
    /// 拓展 Unit 类, 只有战斗才会有的组件跟接口
    /// </summary>
    public partial class Unit : IAwake<int, UnitInfo>, IAwake<UnitInfo, CannonShootInfo>, IDestroy
    {
        public long UnitId;

        /// <summary> Unit Type 直接获取, 不使用 Getter </summary>
        public int Type;

        public NumericComponent AttributeComponent;

        public TransformComponent TransformComponent;

        public FishUnitComponent FishUnitComponent;

        public BulletUnitComponent BulletUnitComponent;

        public PlayerSkillComponent PlayerSkillComponent;

        public Entity GameObjectComponent;

        public Entity BattleUnitViewComponent;

        public Entity AnimatorComponent;
    }
}