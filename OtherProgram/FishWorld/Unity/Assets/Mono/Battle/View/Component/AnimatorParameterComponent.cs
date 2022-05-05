using System.Collections.Generic;

namespace ET
{
    /// <summary> 碰撞体辅助类, 避免反复获取创建相同的碰撞体 </summary>
    public static class AnimatorParameterComponent
    {
        private static Dictionary<int, HashSet<string>> animatorParameterMap;

        static AnimatorParameterComponent() =>
               animatorParameterMap = new Dictionary<int, HashSet<string>>();

        public static void Clear() => animatorParameterMap.Clear();

        public static void Add(int colliderId, HashSet<string> parameters)
        {
            if (!animatorParameterMap.ContainsKey(colliderId))
                animatorParameterMap.Add(colliderId, parameters);
        }

        public static HashSet<string> Get(int colliderId)
        {
            if (animatorParameterMap.ContainsKey(colliderId))
                return animatorParameterMap[colliderId];

            return null;
        }
    }
}