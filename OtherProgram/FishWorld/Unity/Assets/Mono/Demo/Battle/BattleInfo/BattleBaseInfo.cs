 namespace ET
{
    /// <summary>
    /// ILRuntime 调用 Mono 层传输用数据结构基类
    /// </summary>
	public abstract class BattleBaseInfo
    {
        /// <summary>
        /// 放入回收池, 这里使用继承的形式实现, 不用在每个 Helper 里实现对应
        /// BattleBaseInfo 的回收方法, 只需要在热更层调用 info 自身的回收方法即可
        /// </summary>
        public void PushPool() => MonoPool.Instance.Recycle(this);
    }
}