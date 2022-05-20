namespace ET
{
    /// <summary> Mono 层旋转数据类 </summary>
	public class TransformRotateInfo
    {
        public float LocalRotationZ;

        /// <summary> 总旋转时间, 毫秒 </summary>
        public uint RotationDuration;

        /// <summary> 当前旋转时间, 毫秒 </summary>
        public int RotationTime;

        /// <summary> 保持朝向主摄像机 </summary>
        public bool IsFowardMainCamera;

        /// <summary> 是否在播放旋转动画中 </summary>
        public bool IsRotating
        {
            get => RotationDuration > 0;
            set
            {
                if (!value) RotationDuration = 0;
            }
        }
    }
}