namespace ET
{
    /// <summary>
    /// 变换组件辅助类, 对应 ILRuntime 层 TransformComponent 调用方法
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class TransformHelper
    {
        public static TransformInfo PopInfo() =>
                                    MonoPool.Instance.Fetch(typeof(TransformInfo)) as TransformInfo;

        public static void PushPool(TransformInfo info) => MonoPool.Instance.Recycle(info);
    }
}