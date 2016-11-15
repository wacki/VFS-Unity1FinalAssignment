using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG {

    public class FollowCam : MonoBehaviour {

        public GameObject target;
        public float smoothTime = 0.3f;
        public float distance = 15;
        public Vector3 orientation;

        private Vector3 _offset;
        private Vector3 _smoothTarget;
        private Vector3 velocity = Vector3.zero;

        void Awake()
        {
            _smoothTarget = target.transform.position;
        }

        void Update()
        {
            var targetPos = target.transform.position;
            _smoothTarget = Vector3.SmoothDamp(_smoothTarget, targetPos, ref velocity, smoothTime);

            transform.position = _smoothTarget + Quaternion.Euler(orientation) * Vector3.back * distance;
            transform.LookAt(_smoothTarget);
        }

    }

}