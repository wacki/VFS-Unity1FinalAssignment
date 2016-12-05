using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Handles animation of a knight character
    /// </summary>
    public class CharacterAnimation : MonoBehaviour
    {
        //temporary test variables
        public bool getHit;
        public bool roll;
        public float animSpeedPerVelocityFactor = 1.0f;

        private Animator _animator;
        private Rigidbody _rb;
        private PlayerController _pc;

        private NavMeshAgent _agent;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _pc = GetComponent<PlayerController>();

            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            // debug stuff
            if (getHit)
            {
                _animator.SetTrigger("Hit");
                getHit = false;
            }

            if (roll)
            {
                _animator.SetTrigger("roll");
                roll = false;
            }

            // Yea I wanted to get the agents to use rigidbodies but I would've had to 
            // code that myself, sooooo nooooo. Yea This is what I got. Won't do stuff like
            // this on the final project, I promise.
            Vector3 velocity = Vector3.zero;
            if(_agent != null)
            {
                velocity = _agent.velocity;
            }
            else
            {
                velocity = _rb.velocity;
            }

            //_pc.lookDir;
            //_pc.moveDir;
            Vector3 localMovDir = (transform.worldToLocalMatrix * velocity).normalized;
            
            _animator.SetFloat("forward", localMovDir.z);
            _animator.SetFloat("right", localMovDir.x);
            
        }
    }

}