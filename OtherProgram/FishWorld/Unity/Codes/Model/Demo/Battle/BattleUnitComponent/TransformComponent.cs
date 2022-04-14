namespace ET
{
    /// <summary>
    /// 变换组件, 只跑数据逻辑, 真正的设置在 GameObjectComponent 里
    /// 这里存放逻辑相关成员变量, 逻辑数据不在视图层做改变(除了视图层重置 Transform 外)
    /// </summary>
	public class TransformComponent : Entity, IAwake, IDestroy
    {
        /// <summary> 变换结构体信息类 </summary>
        public TransformInfo Info;

        /// <summary>
        /// 脏标记, 防止每次改变都计算一次屏幕坐标, 只在更新时判断是否重新计算
        /// </summary>
        public bool IsScreenPosDirty;

        /// <summary> 是否在屏幕内 </summary>
        public bool IsInScreen;

        /// <summary>
        /// 节点名, 只在编辑器模式下用于设置名字
        /// 通过热更层标记进行判断(原来通过宏定义)
        /// 放 Model 层是不想其他成员变量放 ModelView 层
        /// </summary>
        public string NodeName;
    }
}