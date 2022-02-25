using System.Collections.Generic;
using System.Numerics;

namespace ET
{
    public partial class UnitConfig
    {
        public Vector3 TestValue;
    }

    public class TestVector3
    {
        public Vector3 TestValue;
    }

    public partial class UnitConfigCategory
    {
        public List<TestVector3> TestVector3List = new List<TestVector3>();

        public override void AfterEndInit()
        {
            base.AfterEndInit();

            foreach (var config in this.dict.Values)
            {
                config.TestValue = new Vector3(config.Position, config.Height, config.Weight);
                this.TestVector3List.Add(new TestVector3(){ TestValue = config.TestValue });
            }
        }

        public UnitConfig GetUnitConfigByHeight(int height)
        {
            UnitConfig unitConfig = null;
            foreach (var info in this.dict.Values)
            {
                if (info.Height == height)
                {
                    unitConfig = info;
                    break;
                }
            }
            return unitConfig;
        }
    }
}
