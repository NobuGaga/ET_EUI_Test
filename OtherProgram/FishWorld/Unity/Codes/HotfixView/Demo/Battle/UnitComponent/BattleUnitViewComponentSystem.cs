using ET.EventType;

namespace ET
{
    #region Event

    public class AfterBattleGameObjectCreate_TransformInit : AEvent<AfterBattleGameObjectCreate>
    {
        protected override async ETTask Run(AfterBattleGameObjectCreate args)
        {
            BattleUnitViewComponent battleUnitComponent = args.Unit.BattleUnitViewComponent();
            await battleUnitComponent.InitTransform();
        }
    }

    #endregion

    #region Life Circle

    [ObjectSystem]
    public class BattleUnitViewComponentAwakeSystem : AwakeSystem<BattleUnitViewComponent>
    {
        public override void Awake(BattleUnitViewComponent self)
        {
            Unit unit = self.Parent as Unit;
            switch (unit.UnitType)
            {
                case UnitType.Fish:
                    self.NodeParent = GlobalComponent.Instance.FishRoot;
                    break;
                case UnitType.Bullet:

                    break;
                default:

                    return;
            }
        }
    }

    [ObjectSystem]
    public class BattleUnitViewComponentDestroySystem : DestroySystem<BattleUnitViewComponent>
    {
        public override void Destroy(BattleUnitViewComponent self)
        {
            // Battle TODO
        }
    }

    #endregion

    #region Component Getter

    /// <summary>
    /// BattleUnitViewComponent 跟 Unit 的其他组件进行交互的获取接口实现定义在这里
    /// </summary>
    public static class BattleUnitViewComponentChildComponentSystem
    {
        public static Unit Unit(this BattleUnitViewComponent self) => self.Parent as Unit;

        public static BattleUnitLogicComponent BattleUnitLogicComponent(this BattleUnitViewComponent self)
                                                                => self.Unit().BattleUnitLogicComponent();

        public static TransformComponent TransformComponent(this BattleUnitViewComponent self)
                                                    => self.BattleUnitLogicComponent().TransformComponent();

        public static FishMoveComponent FishMoveComponent(this BattleUnitViewComponent self)
                                                   => self.BattleUnitLogicComponent().FishMoveComponent();

        public static GameObjectComponent GameObjectComponent(this BattleUnitViewComponent self)
                                                        => self.Unit().GetComponent<GameObjectComponent>();
    }

    #endregion

    #region Base Function

    public static class BattleUnitViewComponentSystem
    {
        public static bool IsInitModel(this BattleUnitViewComponent self) => self.GameObjectComponent() != null;

        public static void Update(this BattleUnitViewComponent self)
        {
            FishMoveInfo info = self.FishMoveComponent().Info;
            if (self.Unit().UnitType == UnitType.Fish)
            {
                TransformComponent transformComponent = self.TransformComponent();
                self.SetLocalPos(transformComponent.LogicLocalPos);
                self.SetForward(transformComponent.LogicForward);
            }
        }
    }

    #endregion
}