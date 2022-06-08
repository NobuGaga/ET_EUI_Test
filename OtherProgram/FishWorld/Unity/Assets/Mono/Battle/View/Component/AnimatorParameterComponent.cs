using System.Collections.Generic;

namespace ET
{
    /// <summary> 碰撞体辅助类, 避免反复获取创建相同的碰撞体 </summary>
    public static class AnimatorParameterComponent
    {
        private static Dictionary<string, HashSet<string>> animatorParameterMap;

        static AnimatorParameterComponent() =>
               animatorParameterMap = new Dictionary<string, HashSet<string>>();

        public static void Clear() => animatorParameterMap.Clear();

        public static void Add(string resId, HashSet<string> parameters)
        {
            if (!animatorParameterMap.ContainsKey(resId))
                animatorParameterMap.Add(resId, parameters);
        }

        public static HashSet<string> Get(string resId)
        {
            if (animatorParameterMap.ContainsKey(resId))
                return animatorParameterMap[resId];

            return null;
        }
    }
}