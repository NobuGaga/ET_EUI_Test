using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ET
{
    public class SkillPlayable : BaseAnimation
    {
        private Animator m_animator;

        private string animatorInstanceID;

        private AnimationClip m_curClip;

        private PlayableGraph playableGraph;

        private AnimationPlayableOutput playableOutput;

        private AnimationClipPlayable playableClip;

        public SkillPlayable(Animator animator) => Init(animator);

        public override void Init<T>(T animator)
        {
            m_animator = animator as Animator;
            animatorInstanceID = m_animator.GetInstanceID().ToString();

            playableGraph = PlayableGraph.Create(animatorInstanceID);
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.UnscaledGameTime);
            playableOutput = AnimationPlayableOutput.Create(playableGraph, animatorInstanceID, m_animator);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time)
        {
            m_curClip = clip;

            if (clip == null) return;

            if (playableClip.IsValid())
                playableClip.Destroy();

            playableClip = AnimationClipPlayable.Create(playableGraph, clip);
            playableOutput.SetSourcePlayable(playableClip);
            playableGraph.Play();

            base.SetAnimationPlayTime(clip, time);
        }

        protected override void SampleAnimation()
        {
            if (m_curClip == null)
                return;

            if (!playableClip.IsValid())
                return;

            playableClip.SetTime(m_curPlayTime);
        }

        public override void Dispose()
        {
            if (playableGraph.IsValid())
                playableGraph.Destroy();

            m_animator = null;
            m_curClip = null;
        }
    }
}