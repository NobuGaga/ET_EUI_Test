using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public static class AnimationClipHelper
    {
        private static Dictionary<string, Dictionary<string, AnimationClip>> modelClipMap = 
                                   new Dictionary<string, Dictionary<string, AnimationClip>>();

        private static StringBuilder stringBuilder = new StringBuilder();

        public static void Add(string resId, string assetName, AnimationClip clip)
        {
            stringBuilder.Clear();
            bool isMotionNameStart = false;
            for (int index = 0; index < assetName.Length; index++)
            {
                char @char = assetName[index];
                if (isMotionNameStart)
                {
                    stringBuilder.Append(@char);
                    continue;
                }

                if (!isMotionNameStart && @char == ConstHelper.MotionSymbol)
                    isMotionNameStart = true;
            }

            if (stringBuilder.Length == 0)
                return;

            string clipName = stringBuilder.ToString();
            Dictionary<string, AnimationClip> clipMap;
            if (modelClipMap.ContainsKey(resId))
                clipMap = modelClipMap[resId];
            else
            {
                clipMap = new Dictionary<string, AnimationClip>();
                modelClipMap.Add(resId, clipMap);
            }

            if (clipMap.ContainsKey(clipName))
                clipMap[clipName] = clip;
            else
                clipMap.Add(clipName, clip);
        }

        public static bool Contains(string resId) => modelClipMap.ContainsKey(resId);

        public static AnimationClip GetClip(string resId, string motionName)
        {
            if (!modelClipMap.ContainsKey(resId))
                return null;

            var clipMap = modelClipMap[resId];
            if (!clipMap.ContainsKey(motionName))
                return null;

            return clipMap[motionName];
        }

        public static void Play(GameObject gameObject, string resId, string motionName, bool isLoop)
        {
            var animation = UnityComponentHelper.GetAnimation(gameObject);
            if (animation == null)
                return;

            var clip = GetClip(resId, motionName);
            if (clip == null)
                return;

            animation.Play(clip, isLoop);
        }

        public static void Pause(GameObject gameObject) =>
                           UnityComponentHelper.GetAnimation(gameObject)?.Pause();

        public static void Stop(GameObject gameObject) =>
                   UnityComponentHelper.GetAnimation(gameObject)?.Stop();
    }
}