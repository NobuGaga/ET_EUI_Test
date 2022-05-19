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
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            playableOutput = AnimationPlayableOutput.Create(playableGraph, animatorInstanceID, m_animator);
        }

        public override void Play(AnimationClip clip, bool isLoop) {
            m_curClip = clip;

            if (playableClip.IsValid())
                playableClip.Destroy();

            playableClip = AnimationClipPlayable.Create(playableGraph, clip);
            playableOutput.SetSourcePlayable(playableClip);
            playableGraph.Play();
            
            base.Play(clip, isLoop);
        }

        protected override void SampleAnimation() { }

        protected override void Replay() => Play(m_curClip, isLoop);

        public override void Dispose()
        {
            if (playableGraph.IsValid())
                playableGraph.Destroy();

            m_animator = null;
            m_curClip = null;
        }
    }
}