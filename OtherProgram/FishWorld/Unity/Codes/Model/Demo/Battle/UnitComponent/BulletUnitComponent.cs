namespace ET
{
    // 子弹移动组件, 用来储存子弹相关的数据, 还有跟 Mono 层交互
	public class BulletUnitComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 追踪鱼 Unit ID </summary>
        public long TrackFishUnitId;

        /// <summary>
        /// 子弹参与移动数据传输消息体, 传给 Mono 层然后直接修改里面的值
        /// </summary>
        public BulletMoveInfo Info;
    }
}