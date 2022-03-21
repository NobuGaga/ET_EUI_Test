namespace ET
{
    /// <summary>
    /// ILRuntime 调用 Mono 层传输用数据结构基类
    /// </summary>
	public abstract class BattleBaseInfo
    {
        public void PushPool() => MonoPool.Instance.Recycle(this);
    }
}