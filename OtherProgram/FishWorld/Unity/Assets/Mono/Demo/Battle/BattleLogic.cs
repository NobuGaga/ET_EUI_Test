using UnityEngine;
using ET.Battle;

namespace ET
{
    public static class BattleLogic
    {
        public static void Init()
        {
            ReferenceHelper.Init();

#if UNITY_EDITOR

            ReferenceHelper.GizmosCaller += BattleDebug.OnDrawGizmosCallBack;
#endif
        }

#if UNITY_EDITOR

        public static void AddLineDrawData(Vector3 from, Vector3 to) => BattleDebug.AddLineDrawData(from, to);
#endif
    }
}