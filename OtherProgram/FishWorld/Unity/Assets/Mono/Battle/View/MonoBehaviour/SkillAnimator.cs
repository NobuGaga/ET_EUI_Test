using UnityEngine;

namespace ET {

    internal class SkillAnimator : BaseAnimation {

        private Animator m_animator;
        private string m_playName;
        private float m_frameRate;

        public SkillAnimator(Animator animator) => Init(animator);

        public override void Init<T>(T animator) => m_animator = animator as Animator;

        private void CheckRecord(AnimationClip clip) {
            if (clip.name == m_playName)
                return;
            Record(clip);
        }
        private void Record(AnimationClip clip) {
            m_playName = clip.name;
            m_frameRate = clip.frameRate;
            Record(clip.name, clip.length);
        }

        private void Record(string name, float length) {
            int frameCount = (int)(length * m_frameRate) + 1;
            m_animator.Rebind();
            m_animator.StopPlayback();
            m_animator.Play(name);
            m_animator.recorderStartTime = 0;
            m_animator.StartRecording(frameCount);
            for (int index = 0; index < frameCount; index++)
                m_animator.Update(1 / m_frameRate);
            m_animator.StopRecording();
            m_animator.StartPlayback();
        }

        public override void Play(AnimationClip clip, bool isLoop) {
            CheckRecord(clip);
            base.Play(clip, isLoop);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time) {
            CheckRecord(clip);
            base.SetAnimationPlayTime(clip, time);
        }

        public void SetStateAnimationPlayTime(float time) {
            if (string.IsNullOrEmpty(m_playName)) {
                Debug.LogError("SkillAnimator::Play animator hasn't record before calling play function");
                return;
            }
            if (m_animator.recorderMode != AnimatorRecorderMode.Playback)
                m_animator.StartPlayback();
            base.SetAnimationPlayTime(null, time);
        }

        protected override void SampleAnimation() {
            if (m_animator.recorderMode != AnimatorRecorderMode.Playback)
                return;
            if (m_curPlayTime > m_animator.recorderStopTime)
                m_curPlayTime = m_animator.recorderStopTime;
            m_animator.playbackTime = m_curPlayTime;
            m_animator.Update(1 / m_frameRate);
        }

        public override void Dispose() { }
    }
}