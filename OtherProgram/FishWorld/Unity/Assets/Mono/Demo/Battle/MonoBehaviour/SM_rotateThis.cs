using UnityEngine;

namespace ET
{
    public class SM_rotateThis : MonoBehaviour
    {
        public Vector3 rotationVector;
        private Transform cacheTransform;

	    void Start () => cacheTransform = transform;

	    void Update () => transform.Rotate(rotationVector * Time.deltaTime);
    }
}