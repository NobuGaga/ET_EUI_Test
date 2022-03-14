namespace ET
{
    /// <summary>
    /// 原来自带代码实现了 IAwake<int> 用来复制表 Id
    /// 因为并不是每个战斗实体都有表 Id 所以这里不作赋值跟保存
    /// 转到具体的鱼组件去赋值
    /// </summary>
    [ObjectSystem]
    public class UnitAwakeSystem : AwakeSystem<Unit, UnitInfo>
    {
        // 添加通用数据或者组件
        // 在 AddChild 的时候根据传入参数调用相应的 IAake 方法
        // 在 UnitFactory 进行类型判断对应的使用 AddChild 传入参数
        public override void Awake(Unit self, UnitInfo unitInfo)
        {
            InitAttrData(self, unitInfo);
        }

        private void InitAttrData(Unit self, UnitInfo unitInfo)
        {
            NumericComponent numericComponent = self.AddComponent<NumericComponent>();
            // 改用 var 以免 UnitInfo 改变后要修改别的地方代码
            var numericTypes = unitInfo.Ks;
            var numericValues = unitInfo.Vs;

            if (unitInfo.Ks == null || unitInfo.Vs == null ||
                unitInfo.Ks.Count <= 0 || unitInfo.Vs.Count <= 0)
                return;

            for (int i = 0; i < numericTypes.Count; i++)
                numericComponent.Set(numericTypes[i], numericValues[i]);
        }
    }

    [ObjectSystem]
    public class UnitDestroySystem : DestroySystem<Unit>
    {
        public override void Destroy(Unit self)
        {

        }
    }

    /// <summary>
    /// 每个 Unit 通用都有的数据或者组件都在这里设置
    /// </summary>
    public static class OtherSolutionUnitSystem
    {
        public static void Test(this Unit self)
        {

        }
    }
}