namespace ET
{
    public partial class BattleComponentAwakeSystem : AwakeSystem<BattleComponent>
    {
        public override void Awake(BattleComponent self)
        {
            // Mono 层方法只能在 View 层调用
            BattleLogic.Init();
        }
    }

    public static class BattleComponentSystem
    {
        public static void Test(this BattleComponent self)
        {

        }
    }
}