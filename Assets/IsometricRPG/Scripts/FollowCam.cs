using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Dynamic follow cam with smoothing. 
    /// But still quite a bit of a mess
    /// </summary>
    public class FollowCam : MonoBehaviour
    {

        // target we're following
        public GameObject target;
        // duration until reaching target
        public float smoothTime = 0.3f;
        // distance we should have from the target
        public float distance = 15;
        // orientation to keep the camera in
        public Vector3 orientation;
        // should the camera follow the mouse?
        public bool mouseFollow = true;
        // how far out should the camera go if following the mouse
        public float maxMouseDistance = 5.0f;
        
        private Vector3 _offset;
        private Vector3 _smoothTarget;
        private Vector3 velocity = Vector3.zero;

        // Oh yea, fade objects. IF a fade object is between the camera and the player we will hide it
        // This is the cache of all of them
        private List<FadeOutObject> _cachedFadeObjs = new List<FadeOutObject>();

        void Awake()
        {
            _smoothTarget = target.transform.position;
        }

        void Update()
        {
            var targetPos = target.transform.position;


            if (mouseFollow)
            {
                // follow the mouse position based on where it is on the screen
                Vector3 mousePos = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
                mousePos.x -= 0.5f * Screen.width;
                mousePos.z -= 0.5f * Screen.height;
                mousePos.x /= Screen.width;
                mousePos.z /= Screen.height;

                targetPos += Quaternion.Euler(0, orientation.y, 0) * mousePos * maxMouseDistance;
            }


            // smooth towards the current target
            _smoothTarget = Vector3.SmoothDamp(_smoothTarget, targetPos, ref velocity, smoothTime);

            transform.position = _smoothTarget + Quaternion.Euler(orientation) * Vector3.back * distance;
            // actually look at the target
            transform.LookAt(_smoothTarget);

            // check for intersecting fadeoutobjects
            CheckForIntersecting();
        }

        /// <summary>
        /// If something is blocking our view we want to hide it.
        /// If I had more time I would actually implement something neat that fades in and out but
        /// depth sorting of  transparent stuff would just mess with the look in the current project
        /// So I opted for turning the objects on and off
        /// </summary>
        void CheckForIntersecting()
        {
            // get a few important vars
            var targetPos = target.transform.position;// + target.GetComponent<Rigidbody>().velocity;
            var dir = targetPos - transform.position;
            var distance = dir.magnitude;
            dir.Normalize();

            // raycast hit info
            var hitInfo = Physics.RaycastAll(transform.position, dir, distance);

            // list of fade out objects to remove from the cache 
            List<FadeOutObject> toRemove = new List<FadeOutObject>(_cachedFadeObjs);

            // go over all the hit objects
            foreach (var hit in hitInfo)
            {
                var fadeObj = hit.collider.GetComponent<FadeOutObject>();
                if (fadeObj == null)
                    continue;

                // Current object is not in our cache
                if (!_cachedFadeObjs.Contains(fadeObj))
                {
                    // fade it out 
                    fadeObj.FadeOut();
                    // add it to the cache
                    _cachedFadeObjs.Add(fadeObj);
                }
                else
                {
                    // Current object is already in our cache, so remove it from the cleanup list
                    toRemove.Remove(fadeObj);
                }
            }

            // clear cached objects
            foreach (var fadeObj in toRemove)
            {
                // fade back in
                fadeObj.FadeIn();
                // remove from cache
                _cachedFadeObjs.Remove(fadeObj);
            }
        }
    }

}