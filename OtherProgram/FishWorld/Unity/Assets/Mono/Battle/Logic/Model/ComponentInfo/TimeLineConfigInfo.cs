namespace ET
{
    /// <summary> 时间轴通用表数据结构 </summary>
	public class TimeLineConfigInfo
    {
        /// <summary>
        /// 生命周期节点, 具体为鱼线运动时间百分比
        /// 或者是生命周期总时长百分比
        /// 上一个节点 IsAutoNext 为 true 时
        /// 不按照这个百分比执行
        /// 按照自动执行队列执行时间轴节点
        /// </summary>
        public float LifeTime;

        /// <summary> 时间轴类型 </summary>
        public int Type;

        /// <summary> 
        /// 执行完当前节点是否自动执行下一个节点
        /// 需要知道当前节点的执行时间
        /// </summary>
        public bool IsAutoNext;

        /// <summary> 执行节点时长 (秒) </summary>
        public float ExecuteTime;

        /// <summary> 自动执行节点标识 </summary>
        public bool IsAutoExecute;

        /// <summary> 剩余参数缓存 </summary>
        public string[] Arguments;

        public TimeLineConfigInfo(float lifeTime, int type)
        {
            LifeTime = lifeTime / 100;
            Type = type;
            IsAutoNext = false;
            ExecuteTime = 0;
            IsAutoExecute = false;
        }
    }
}