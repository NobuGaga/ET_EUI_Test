using UnityEngine;

namespace ET
{
    public class SM_rotateThis : MonoBehaviour
    {
        public Vector3 rotationVector;

	    void Update () => transform.Rotate(rotationVector * Time.deltaTime);
    }
}