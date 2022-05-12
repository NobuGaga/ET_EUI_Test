using UnityEngine;

namespace ET
{
    public abstract class BaseAnimation
    {
        private bool m_isPlaying;
        public bool IsPlaying => m_isPlaying;

        private bool m_isPlayOver;
        public bool IsPlayOver => m_isPlayOver;
        
        protected float m_curPlayTime;
        public float PlayTime => m_curPlayTime;

        protected bool isLoop;

        protected float m_clipLength;

        public abstract void Init<T>(T data);

        public virtual void Reset()
        {
            m_isPlaying = false;
            m_isPlayOver = true;
            m_curPlayTime = 0;
            isLoop = false;
        }

        public virtual void Play(AnimationClip clip, bool isLoop) {
            m_isPlaying = true;
            m_isPlayOver = false;
            m_curPlayTime = 0;
            m_clipLength = clip.length;
            this.isLoop = isLoop;
        }

        public void Pause()
        {
            if (m_isPlaying)
            {
                m_isPlaying = false;
                return;
            }

            if (m_isPlayOver && !isLoop)
                return;

            m_isPlaying = true;
        }

        public void Stop() {
            m_isPlaying = false;
            m_isPlayOver = true;
            m_curPlayTime = 0;
            SampleAnimation();
        }

        public virtual void SetAnimationPlayTime(AnimationClip clip, float time) {
            m_curPlayTime = time;
            SampleAnimation();
        }

        public void Update(float deltaTime) {
            if (!m_isPlaying)
                return;
            m_curPlayTime += deltaTime;
            if (m_curPlayTime > m_clipLength) {
                m_curPlayTime = m_clipLength;
                SampleAnimation();
                if (isLoop)
                {
                    Replay();
                    m_curPlayTime = 0;
                }
                else
                    Stop();
            }
            else
                SampleAnimation();
        }

        protected virtual void Replay()
        {

        }

        protected abstract void SampleAnimation();

        public static bool IsGenericState(AnimationClip clip) {
            return !clip.legacy && !clip.humanMotion;
        }

        public abstract void Dispose();
    }
}