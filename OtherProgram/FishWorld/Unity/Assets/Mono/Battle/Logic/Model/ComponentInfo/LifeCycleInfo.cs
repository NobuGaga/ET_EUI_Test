// Battle Review Before Boss Node

namespace ET
{
    /// <summary>
    /// 生命周期数据结构, 使用 class 实现, 避免交互时频繁结构体拷贝
    /// 需要在热更层释放时将引用返回数据缓存池
    /// </summary>
	public class LifeCycleInfo
    {
        /// <summary> 当前生命周期时间, 已存活时间除以生命周期长度 </summary>
        public float CurrentLifeTime;

        /// <summary> 已存活时间 (秒) </summary>
        public float SurvivalTime;

        /// <summary> 总存活时间 (秒) </summary>
        public float TotalLifeTime;

        public void Reset()
        {
            CurrentLifeTime = 0;
            SurvivalTime = 0;
            TotalLifeTime = 0;
        }

        /// <summary> 是否到达生命周期尽头 </summary>
        public bool IsLifeTimeOut => SurvivalTime > TotalLifeTime || CurrentLifeTime > 1;
    }
}