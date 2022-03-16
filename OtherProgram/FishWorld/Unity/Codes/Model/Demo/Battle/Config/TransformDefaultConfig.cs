using UnityEngine;

namespace ET
{
	public static class TransformDefaultConfig
	{
        public static Vector3 DefaultPosition = Vector3.zero;
        public static Quaternion DefaultRotation = Quaternion.identity;
        public static Vector3 DefaultScale = new Vector3(0.1f, 0.1f, 0.1f);
        public static Vector3 DefaultForward = Vector3.forward;
        public static Vector3 DefaultScreenPos = new Vector3(-100, -100, -100);
        public const string DefaultName = "DefaultName";
    }
}