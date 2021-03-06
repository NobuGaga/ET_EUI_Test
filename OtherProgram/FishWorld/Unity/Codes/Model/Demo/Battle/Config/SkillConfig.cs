using UnityEngine;

namespace ET
{
    public static class SkillType
    {
        /// <summary> 瞄准 </summary>
        public const int Aim = 1;

        /// <summary> 冰冻 </summary>
        public const int Ice = 2;
        
        /// <summary> 激光(狂暴) </summary>
        public const int Laser = 3;
    }

    public static class SkillConfig
    {
        /// <summary> 镭射线预设结束节点名 </summary>
        public const string LaserEndNodeName = "end";

        public static Vector3 RemovePoint = new Vector3(-20000, 0, 0);
    }
}