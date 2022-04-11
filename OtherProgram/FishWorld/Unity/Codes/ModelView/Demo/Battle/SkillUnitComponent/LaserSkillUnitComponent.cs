using UnityEngine;

namespace ET
{
	/// <summary> 镭射技能组件, 用来控制瞄准技能表现 </summary>
	public class LaserSkillUnitComponent : Entity, IAwake<Transform>, IDestroy
	{
		/// <summary> 炮台射击点 </summary>
		public Transform CannonShootPointNode;

		/// <summary> 镭射线结束节点 </summary>
		public Transform EndNode;
	}
}