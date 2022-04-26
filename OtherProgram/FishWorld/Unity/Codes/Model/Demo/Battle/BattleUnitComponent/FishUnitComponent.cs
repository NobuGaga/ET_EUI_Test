// Battle Review Before Boss Node

namespace ET
{
    /// <summary> 鱼移动组件, 用来储存鱼相关的数据, 还有跟 Mono 层交互 </summary>
	public class FishUnitComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 鱼参与移动数据传输消息体, 传给 Mono 层然后直接修改里面的值 </summary>
        public FishMoveInfo MoveInfo;

        /// <summary> Mono 层鱼屏幕数据类 </summary>
        public FishScreenInfo ScreenInfo;
    }
}