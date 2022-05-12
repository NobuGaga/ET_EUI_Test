using System.Collections.Generic;

namespace ET
{
	public static class MotionType
	{
		public const int None = 0;
		public const int Idle = 1;
		public const int Run = 2;
		public const int Move = 3;
		public const int Attack = 4;
		public const int Die = 5;
	}

	public static class MotionTypeHelper
	{
		private static List<string> motionTypeList = new List<string>
		{
			"None",
			"Idle",
			"Run",
			"move",
			"attack",
			"die",
		};

		public static string Get(int motionType) => motionTypeList[motionType];
	}

	/// <summary> AnimatorComponent 默认数据数据 </summary>
	public static class AnimatorConfig
	{
		public const ushort DefaultStopSpeed = 1;

		public const ushort DefaultMontionSpeed = 1;

		public const int DefaultMotionType = MotionType.Move;
	}
}