/// <summary>
/// 用来拓展 OuterMessage 消息类型的类
/// </summary>

namespace ET
{
	public partial class UnitInfo
	{
		public UnitType UnitType => (UnitType)Type;
	}
}