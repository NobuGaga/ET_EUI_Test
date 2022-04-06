namespace ET
{
    /// <summary> 
    /// 视图层拓展 CannonComponent 方法
    /// CannonComponent 挂在 Player 类型的 Unit 下
    /// </summary>
    public static class CannonComponenViewtSystem
    {
        public static void PlayAnimation(this CannonComponent self)
        {
            AnimatorComponent animatorComponent = self.Cannon.GetComponent<AnimatorComponent>();
            animatorComponent.Play(MotionType.attack);
        }
    }
}