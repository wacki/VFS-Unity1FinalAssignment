using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Wacki.IsoRPG
{
    /// <summary>
    /// Class representing the player character
    /// </summary>
    public class PlayerController : BaseUnit
    {
        // Hi Ivo, you said we need a static variable right? Yea so this is it.
        public static int failCounter = 0;

        /// <summary>
        /// I actually wanted to get a dodge roll in here but it got pushed back
        /// And in the end I didn't have time. But I'll add it for the game mechanics thing
        /// 
        /// Edit: Scratch that I just implemented a shitty dodge roll version because why not. 
        /// Way more fun now
        /// </summary>
        public enum State
        {
            Default,
            Roll
        }

        /// <summary>
        /// Current state of the player character
        /// </summary>
        public State state { get { return _state; } }
        private State _state;
        
        // max acceleration of the player character
        public float moveAcceleration;
        // velocity is capped to maxVelocity
        public float maxVelocity;
        // how long does a roll take
        public float rollDuration;
        // what velocity does the playe rhave during a roll
        public float rollVelocity;
        // factor to dynamically scale animation speed
        public float animSpeedPerVelocityFactor = 1.0f;
        // distance where enemies will be hit
        public float hitDistance;
        // special UI element notifying the player about death
        public Text deathTextUI;

        public int rollEnergyCost;

        // this should be in the base unit but yea. Tells us if the player is currently dead
        private bool dying;

        private float _rollTimer;
        

        private Vector3 _rollDir;


        protected override void Update()
        {
            base.Update();
            if (dying)
            {
                return;
            }

            UpdateLookAt();
            UpdateMovement();
            Animate();
        }

        /// <summary>
        /// Look at the mouse
        /// </summary>
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

        // move the player
        private void UpdateMovement()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");

            var viewSpaceInput = new Vector3(h, 0, v);

            viewSpaceInput = Camera.main.transform.rotation * viewSpaceInput;
            viewSpaceInput = Vector3.ProjectOnPlane(viewSpaceInput, Vector3.up);
            // cap the input vector to unit length but ensure we keep analog precision
            float inputMag = Mathf.Min(viewSpaceInput.magnitude, 1.0f);
            viewSpaceInput.Normalize();
            viewSpaceInput *= inputMag;

            Debug.DrawLine(transform.position, transform.position + viewSpaceInput, Color.red);

            //var vel = _rb.velocity;
            //vel.x = viewSpaceInput.x;
            //vel.z = viewSpaceInput.z;
            //_rb.velocity = vel;

            if (_state == State.Default)
            {
                var vel = _rb.velocity;
                vel += viewSpaceInput * moveAcceleration * Time.deltaTime;
                if (vel.magnitude > maxVelocity)
                    vel = vel.normalized * maxVelocity;

                _rb.velocity = vel;
            }
            else
            {
                var vel = _rb.velocity;
                    vel = _rollDir * rollVelocity;
                _rb.velocity = vel;

                transform.forward = _rollDir;

                _rollTimer -= Time.deltaTime;
                if (_rollTimer < 0.0f)
                {
                    _state = State.Default;
                }
            }

            // Yea I know attack is in UpdateMovement, sue me!
            if(Input.GetButtonDown("Attack"))
            {
                Attack();
            }

            if (Input.GetButtonDown("Dodge"))
                Roll(viewSpaceInput);

        }

        protected override void TakeDamage(int amount)
        {
            // don't take damage while rolling
            if (state == State.Roll)
                return;


            base.TakeDamage(amount);
        }

        // Animate the player based on his current velocity
        private void Animate()
        {
            Vector3 localMovDir = (transform.worldToLocalMatrix * _rb.velocity) * animSpeedPerVelocityFactor;
            
            _animator.SetFloat("forward", localMovDir.z);
            _animator.SetFloat("right", localMovDir.x);            
        }

        // :(
        private void Roll(Vector3 dir)
        {
            if (state == State.Roll || energy < rollEnergyCost)
                return;

            if (dir.sqrMagnitude < Mathf.Epsilon)
                dir = transform.forward;

            SpendEnergy(rollEnergyCost);

            _animator.SetTrigger("roll");

            Debug.Log("ROLLING");
            _rollTimer = rollDuration;
            _state = State.Roll;
            _rollDir = dir;
        }

        // Handle custom death stuff
        public override void Kill()
        {
            base.Kill();
            _animator.SetTrigger("die");
            dying = true;

            // reload scene after death
            StartCoroutine(ReloadScene());
        }

        IEnumerator ReloadScene()
        {
            // Custom death message
            failCounter++;
            string deathText = "You died " + failCounter + " times!";
            if (failCounter > 30)
                deathText = "You died over 30 times, why are you still playing this shitty game?";
            if (failCounter == 20)
                deathText = "Your 20th death, still going strong I see, don't give up.";
            if (failCounter == 4)
                deathText = "Yea I lied, there is no quit button...";
            if (failCounter == 3)
                deathText = "Maybe this game just isn't for you? The quit button is right there.";
            else if (failCounter == 2)
                deathText = "You died again!";
            else if (failCounter == 1)
                deathText = "You just died!";


            // Activate death text
            deathTextUI.gameObject.SetActive(true);
            float reloadTimer = 10.0f;
            while(reloadTimer > 0)
            {

                deathTextUI.text = deathText + "\nRespawn in " + reloadTimer + " seconds";
                yield return new WaitForSeconds(1.0f);
                reloadTimer -= 1.0f;
            }

            deathTextUI.text = deathText + "\nRespawn in " + reloadTimer + " seconds";

            // we wait an other second so we actually display the zero
            yield return new WaitForSeconds(1.0f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

}