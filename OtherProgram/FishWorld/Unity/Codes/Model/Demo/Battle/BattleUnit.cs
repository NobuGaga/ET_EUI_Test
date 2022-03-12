namespace ET.Battle
{
    public class BattleUnit : Entity, IAwake<int>
    {
        public int ConfigId; //配置表id

        public UnitType UnitType { set; get; }
    }
}