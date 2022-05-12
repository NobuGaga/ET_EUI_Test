using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public static class AnimationClipHelper
    {
        private static Dictionary<int, Dictionary<string, AnimationClip>> modelClipMap = 
                                   new Dictionary<int, Dictionary<string, AnimationClip>>();

        private static StringBuilder stringBuilder = new StringBuilder();

        public static void Add(int configId, string assetName, AnimationClip clip)
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
            if (modelClipMap.ContainsKey(configId))
                clipMap = modelClipMap[configId];
            else
            {
                clipMap = new Dictionary<string, AnimationClip>();
                modelClipMap.Add(configId, clipMap);
            }

            if (clipMap.ContainsKey(clipName))
                clipMap[clipName] = clip;
            else
                clipMap.Add(clipName, clip);
        }

        public static bool Contains(int configId) => modelClipMap.ContainsKey(configId);

        public static void Play(GameObject gameObject, int configId, string motionName, bool isLoop)
        {
            var animation = UnityComponentHelper.GetAnimation(gameObject);
            if (animation == null)
                return;

            if (!modelClipMap.ContainsKey(configId))
                return;

            var clipMap = modelClipMap[configId];
            if (!clipMap.ContainsKey(motionName))
                return;

            var clip = clipMap[motionName];
            //var unityReferense = UnityComponentHelper.GetUnityReferense(gameObject.GetInstanceID());
            //for (int index = 0; index < unityReferense.clipList.Count; index++)
            //{
            //    if (unityReferense.clipList[index].name.Contains(motionName))
            //    {
            //        clip = unityReferense.clipList[index];
            //        break;
            //    }
            //}

            animation.Play(clip, isLoop);
        }

        public static void Pause(GameObject gameObject) =>
                           UnityComponentHelper.GetAnimation(gameObject)?.Pause();

        public static void Stop(GameObject gameObject) =>
                   UnityComponentHelper.GetAnimation(gameObject)?.Stop();
    }
}