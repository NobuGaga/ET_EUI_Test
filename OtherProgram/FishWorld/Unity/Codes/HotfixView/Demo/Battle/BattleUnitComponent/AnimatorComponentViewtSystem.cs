namespace ET
{
    internal static class AnimatorComponentViewtSystem
    {
        internal static void Reset(this AnimatorComponent self)
        {
            self.isStop = false;
            self.stopSpeed = AnimatorConfig.DefaultStopSpeed;
            self.MontionSpeed = AnimatorConfig.DefaultMontionSpeed;
            self.Animator.speed = AnimatorConfig.DefaultMontionSpeed;
        }
    }
}