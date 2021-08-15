using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MyCamera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraModel cameraModel;

        private Vector3 cachedDesiredPosition;

        public void Start()
        {
            cachedDesiredPosition = transform.localPosition;

            Observable.EveryUpdate()
            .Where(_ => !cameraModel.GetTarget())
            .Subscribe(_ =>
            {
                var moveVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                if(moveVec != Vector3.zero)
                {
                    cachedDesiredPosition = transform.localPosition + moveVec;
                }
                var smoothPosition = Vector3.Lerp(transform.localPosition, cachedDesiredPosition, cameraModel.GetSmoothSpeed() * Time.deltaTime);
                transform.localPosition = smoothPosition;
            });
        }
    }
}