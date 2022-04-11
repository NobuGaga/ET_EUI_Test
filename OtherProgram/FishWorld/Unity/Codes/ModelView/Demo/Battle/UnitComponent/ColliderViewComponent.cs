namespace ET
{
	/// <summary> 碰撞体视图组件, 用来创建 Mono 层的碰撞组件 </summary>
	public class ColliderViewComponent : Entity, IAwake, IDestroy
	{
		public bool IsBullet;

		public ColliderMonoComponent MonoComponent;
	}
}