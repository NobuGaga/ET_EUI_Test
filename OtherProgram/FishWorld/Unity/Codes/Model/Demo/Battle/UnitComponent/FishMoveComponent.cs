namespace ET
{
    // 鱼移动组件, 用来储存鱼相关的数据, 还有跟 Mono 层交互
	public class FishMoveComponent : Entity, IAwake<int>, IDestroy
    {
        /// <summary> 鱼基础表 ID, UnitConfig ID </summary>
        public ushort ConfigId;

        /// <summary>
        /// 鱼参与移动数据传输消息体, 传给 Mono 层然后直接修改里面的值
        /// </summary>
        public FishMoveInfo Info;
    }
}