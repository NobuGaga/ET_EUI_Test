namespace ET
{
    /// <summary>
    /// 结构体辅助类
    /// 用于处理热更层需要用到结构体成员变量的环境
    /// 私有方法使用静态拓展, 公有方法使用传参
    /// </summary>
    public static class StructureHelper
    {
        public static Vector3_Class Pop_Vector3_Class() =>
                                    MonoPool.Instance.Fetch(typeof(Vector3_Class)) as Vector3_Class;

        public static void PushPool(Vector3_Class info) => MonoPool.Instance.Recycle(info);
    }
}