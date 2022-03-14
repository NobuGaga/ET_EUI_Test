    namespace ET.Battle
{
    [ObjectSystem]
    public class BattleUnitAwakeSystem : AwakeSystem<BattleUnit, UnitInfo>
    {
        // 添加通用数据或者组件
        public override void Awake(BattleUnit self, UnitInfo unitInfo)
        {
            InitAttrData(self, unitInfo);
        }

        private void InitAttrData(BattleUnit self, UnitInfo unitInfo)
        {
            NumericComponent numericComponent = self.AddComponent<NumericComponent>();
            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var numericTypes = unitInfo.Ks;
            var numericValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;
            
            for (int i = 0; i < numericTypes.Count; ++i)
                numericComponent.Set(numericTypes[i], numericValues[i]);
        }
    }

    [ObjectSystem]
    public class BattleUnitDestroySystem : DestroySystem<BattleUnit>
    {
        public override void Destroy(BattleUnit self)
        {

        }
    }

    /// <summary>
    /// 每个 Battle Unit 通用都有的数据或者组件都在这里设置
    /// </summary>
    public static class BattleUnitSystem
    {
        public static void Test(this BattleUnit self)
        {

        }
    }
}