using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ET
{
    public class SkillPlayable : BaseAnimation
    {
        private Animator m_animator;

        private AnimationClip m_curClip;

        private PlayableGraph playableGraph;

        private AnimationPlayableOutput m_output;

        public SkillPlayable(Animator animator) => Init(animator);

        public override void Init<T>(T animator)
        {
            m_animator = animator as Animator;
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time)
        {
            //AnimationPlayableUtilities.PlayClip(m_animator, clip, out playableGraph);
            //playableGraph.Evaluate(time);
            base.SetAnimationPlayTime(clip, time);
        }

        public override void Play(AnimationClip clip, bool isLoop) {
            //AnimationPlayableUtilities.PlayClip(m_animator, clip, out playableGraph);
            m_curClip = clip;
            if (playableGraph.IsValid())
                playableGraph.Destroy();
            playableGraph = PlayableGraph.Create("PlayAnimationSample");
            // 创建一个Output节点，类型是Animation，名字是Animation，目标对象是物体上的Animator组件
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", m_animator);
            // 创建一个动画剪辑Playable，将clip传入进去
            var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
            // 将playable连接到output
            playableOutput.SetSourcePlayable(clipPlayable);
            playableGraph.Play();
            base.Play(clip, isLoop);
        }

        protected override void SampleAnimation() { }

        protected override void Replay()
        {
            playableGraph.Destroy();
            Play(m_curClip, isLoop);
        }

        public override void Dispose()
        {
            if (playableGraph.IsValid())
                playableGraph.Destroy();
            m_curClip = null;
        }
    }
}