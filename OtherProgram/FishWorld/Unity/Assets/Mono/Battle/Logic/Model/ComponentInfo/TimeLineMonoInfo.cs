namespace ET
{
    /// <summary> 时间轴 Mono 层运行时数据结构 </summary>
	public class TimeLineMonoInfo
    {
        /// <summary> 时间轴配置表 ID </summary>
        public int ConfigId;

        /// <summary> 当前生命周期时间 </summary>
        public float LifeTime;

        /// <summary> 当前时间轴节点索引 </summary>
        public int NodeIndex;

        public void Reset()
        {
            ConfigId = 0;
            LifeTime = 0;
            NodeIndex = 0;
        }
    }
}