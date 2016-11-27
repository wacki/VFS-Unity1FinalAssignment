using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{
    public class CharacterAnimation : MonoBehaviour
    {
        //temporary test variables
        public bool getHit;
        public bool roll;

        private Animator _animator;
        private Rigidbody _rb;
        private PlayerController _pc;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _pc = GetComponent<PlayerController>();
        }

        void Update()
        {
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

            //_pc.lookDir;
            //_pc.moveDir;
            Vector3 velocityLocal = transform.worldToLocalMatrix * _rb.velocity;

            Debug.Log(velocityLocal);
            _animator.SetFloat("forward", velocityLocal.z * 5);
            _animator.SetFloat("right", velocityLocal.x * 5);
        }
    }

}