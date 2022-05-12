namespace ET
{
    /// <summary> 时间轴通用表数据结构 </summary>
	public class TimeLineConfigInfo
    {
        /// <summary> 生命周期节点 </summary>
        public float LifeTime;

        /// <summary> 时间轴类型 </summary>
        public int Type;

        /// <summary> 剩余参数缓存 </summary>
        public string[] Arguments;

        public TimeLineConfigInfo(float lifeTime, int type)
        {
            LifeTime = lifeTime;
            Type = type;
        }
    }
}