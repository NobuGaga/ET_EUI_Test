#if UNITY_EDITOR

using UnityEngine;

namespace ET
{
    public class GizmosCaller : MonoBehaviour
    {

        public delegate void OnDrawGizmosFunction();
        private OnDrawGizmosFunction function;

        private void OnDrawGizmos()
        {
            //function?.Invoke();
            if (function != null)
                function();
        }

        public static GizmosCaller operator +(GizmosCaller caller, OnDrawGizmosFunction function)
        {
            caller.function += function;
            return caller;
        }
    }
}

#endif