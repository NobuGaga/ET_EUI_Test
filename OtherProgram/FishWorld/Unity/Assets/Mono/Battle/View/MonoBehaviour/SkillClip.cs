using UnityEngine;

namespace ET
{
    public class SkillClip : BaseAnimation
    {
        private GameObject m_gameObject;

        private AnimationClip m_curClip;

        public SkillClip(GameObject gameObject) => Init(gameObject);

        public override void Init<T>(T gameObject) => m_gameObject = gameObject as GameObject;

        public override void Play(AnimationClip clip, bool isLoop) {
            m_curClip = clip;
            base.Play(clip, isLoop);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time) {
            m_curClip = clip;
            base.SetAnimationPlayTime(clip, time);
        }

        protected override void SampleAnimation() => m_curClip?.SampleAnimation(m_gameObject, m_curPlayTime);

        public override void Dispose() => m_curClip = null;
    }
}