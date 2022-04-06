#if UNITY_EDITOR

using System;
using UnityEngine;

namespace ET
{
    public class GizmosCaller : MonoBehaviour
    {
        private Action drawGizmos;

        public void OnDrawGizmos()
        {
            //drawGizmos?.Invoke();
            if (drawGizmos != null)
                drawGizmos();
        }

        public static GizmosCaller operator +(GizmosCaller caller, Action drawGizmos)
        {
            caller.drawGizmos += drawGizmos;
            return caller;
        }
    }
}

#endif