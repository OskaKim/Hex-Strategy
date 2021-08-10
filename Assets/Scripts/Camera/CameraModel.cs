using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCamera
{
    public class CameraModel : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 20.0f;
        [SerializeField] private Vector3 followOffset = new Vector3(0, 0, 0);

        public Transform GetTarget() { return target; }
        public void SetTarget(Transform target) { this.target = target; }
        public void RemoveTarget() { this.target = null; }
        public float GetSmoothSpeed() { return smoothSpeed; }
        public Vector3 GetFollowOffset() { return followOffset; }
    }
}