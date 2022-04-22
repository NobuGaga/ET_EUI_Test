// Battle Review Before Boss Node

namespace ET
{
    /// <summary> 鱼移动组件, 用来储存鱼相关的数据, 还有跟 Mono 层交互 </summary>
	public class FishUnitComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 鱼参与移动数据传输消息体, 传给 Mono 层然后直接修改里面的值 </summary>
        public FishMoveInfo Info;

        /// <summary> 瞄准点, 用来定位瞄准技能特效跟, 撒网特效的点(屏幕坐标) </summary>
        public Vector3_Class AimPoint;

        /// <summary> 是否在屏幕内, 这个标记暂时只有鱼才有用, 所以放到这里 </summary>
        public bool IsInScreen;
    }
}