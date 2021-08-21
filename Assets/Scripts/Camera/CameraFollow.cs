using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MyCamera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private CameraModel cameraModel;

        private void Start()
        {
            // TODO : 타겟으로 이동
            //Observable.EveryLateUpdate()
            //.Where(_ => cameraModel.GetTarget())
            //.Subscribe(_ =>
            //{
            //    var targetPos = cameraModel.GetTarget().position;
            //    var desiredPosition = new Vector3(targetPos.x, transform.position.y, targetPos.y) + cameraModel.GetFollowOffset();
            //    var smoothPosition = Vector3.Lerp(transform.position, desiredPosition, cameraModel.GetSmoothSpeed() * Time.deltaTime);
            //    transform.position = smoothPosition;
            //});
        }
    }
}