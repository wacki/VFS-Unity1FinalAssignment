using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    public class PlayerController : MonoBehaviour
    {
        public enum State
        {
            Default,
            Roll
        }

        public State state { get { return _state; } }
        private State _state;

        public Transform tempLockOnPoint;

        public float moveAcceleration;
        public float maxVelocity;
        public float rollDuration;
        public float rollVelocity;

        private Rigidbody _rb;
        private Animator _animator;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateLookAt();
            UpdateMovement();
            UpdateAttack();
        }

        private void UpdateLookAt()
        {
            // resolve look direction
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // create a ground plane on the character's height
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            float hitDistance;

            if (groundPlane.Raycast(ray, out hitDistance))
            {
                transform.LookAt(ray.GetPoint(hitDistance));
            }
        }

        private void UpdateMovement()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");

            var viewSpaceInput = new Vector3(h, 0, v);

            viewSpaceInput = Camera.main.transform.rotation * viewSpaceInput;
            viewSpaceInput = Vector3.ProjectOnPlane(viewSpaceInput, Vector3.up);
            viewSpaceInput.Normalize();
            Debug.DrawLine(transform.position, transform.position + viewSpaceInput, Color.red);

            viewSpaceInput *= maxVelocity;

            var vel = _rb.velocity;
            vel.x = viewSpaceInput.x;
            vel.z = viewSpaceInput.z;
            _rb.velocity = vel;
        }

        private void Roll()
        {

        }

        private void UpdateAttack()
        {
        }

    }

}