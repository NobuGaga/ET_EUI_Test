// Battle Review Before Boss Node

using UnityEngine;

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
    }
}