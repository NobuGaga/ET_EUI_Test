using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public struct UnityReferense
    {
        public GameObject GameObject;

        public Animator Animator;

        public BaseAnimation Animation;

        public UnityReferense(GameObject gameObject)
        {
            GameObject = gameObject;
			Animator = gameObject.GetComponentInChildren<Animator>();
            Animation = new SkillPlayable(Animator);
        }
    }

    /// <summary> 碰撞体辅助类, 避免反复获取创建相同的碰撞体 </summary>
    public static class UnityComponentHelper
    {
        private static Dictionary<int, UnityReferense> referenseMap;

        static UnityComponentHelper() =>
               referenseMap = new Dictionary<int, UnityReferense>(ConstHelper.FisheryUnitCount);

        public static void Clear() => referenseMap.Clear();

        public static void Add(GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            if (!referenseMap.ContainsKey(instanceID))
                referenseMap.Add(instanceID, new UnityReferense(gameObject));
        }

        public static UnityReferense GetUnityReferense(int instanceID)
        {
            referenseMap.TryGetValue(instanceID, out UnityReferense referense);
            return referense;
        }

        public static GameObject GetGameObject(int instanceID)
        {
            referenseMap.TryGetValue(instanceID, out UnityReferense referense);
            return referense.GameObject;
        }

        public static Animator GetAnimator(GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            referenseMap.TryGetValue(instanceID, out UnityReferense referense);
            return referense.Animator;
        }

        public static BaseAnimation GetAnimation(GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            referenseMap.TryGetValue(instanceID, out UnityReferense referense);
            return referense.Animation;
        }

        public static void RemoveGameObject(int instanceID)
        {
            if (!referenseMap.ContainsKey(instanceID))
                return;

            UnityReferense referense = referenseMap[instanceID];
            referense.Animation.Dispose();
        }
    }
}