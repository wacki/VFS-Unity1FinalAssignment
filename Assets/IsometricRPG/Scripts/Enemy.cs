using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{
    /// <summary>
    /// Class for AI enemies
    /// </summary>
    public class Enemy : BaseUnit
    {
        [Header("Enemy specific fields")]
        // patrol path to walk on
        public PatrolPath patrolPath;
        // range at which player is detected 
        public float detectRange;
        // threshold to determine if a waypoint was reached
        public float wpReachedDistanceThreshold;
        // when should the enemy try to attack
        public float attackRange;
        // speed while chasing the player
        public float chaseSpeed;
        // speed while patrolling
        public float patrolSpeed;
        // wait time until giving up on a chase after losing sight of the player
        public float maxBlindChaseTime = 2.0f;
        // current timer telling us how lomg we havent spotted the player
        private float _blindChaseTimer;

        // enemies get a special audio clip when they detect the player
        public AudioClip[] playerDetectClips;

        // enemies can have two states, either chasing or patrolling
        private enum State
        {
            Patrolling,
            Chasing
        }
        // Also, I warn you the code below is a MESS that I've written faster than I can think...
        private State _state;

        // reference to the nav mesh agent
        private NavMeshAgent _agent;
        // refernece to the player
        private PlayerController _player;
        // flag if the enemy is currently dying
        private bool dying;


        public LayerMask layerMask;

        protected override void Kill()
        {
            base.Kill();
            //Destroy(gameObject);
            _animator.SetTrigger("die");
            dying = true;
            _agent.enabled = false;
            StartCoroutine(Dissolve());
        }

        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(patrolPath.currentWaypoint.transform.position);
            _state = State.Patrolling;
            _player = FindObjectOfType<PlayerController>();
            dying = false;
            
        }

        protected override void Update()
        {
            base.Update();
            if (dying)
            {
                return;
            }
   
            // get center of player and this enemy. Used later for a "is in sight" raycast
            var origin = GetComponent<CapsuleCollider>().bounds.center;
            var playerCenter = _player.GetComponent<CapsuleCollider>().bounds.center;

            // We see if anything is obstructing our view to the player
            RaycastHit hitInfo;
            Physics.Raycast(origin, (playerCenter - origin).normalized, out hitInfo, 1000, layerMask);

            // set the playerInSight bool
            bool playerInSight = false;
            if(hitInfo.collider != null)
                playerInSight = hitInfo.collider.gameObject == _player.gameObject;

            // also show the in sight status in the editor so we know what's up
            Debug.DrawLine(origin, playerCenter, (playerInSight ? Color.green:Color.red));

            // calculate distance from player
            float distanceToPlayer = Vector3.Distance(origin, playerCenter);

            // If the playe ris in sight then we want to restart the last sighting timer
            if(playerInSight)
            {
                _blindChaseTimer = maxBlindChaseTime;
            }

            // if player is in sight and we're in the detect range AAAND the player isn't dead we want to chase!
            if (playerInSight && distanceToPlayer < detectRange && !_player.IsDead())
            {
                if (_state != State.Chasing)
                {
                    // play the detect sound if we just switched to the chasing state
                    PlayRandomClip(playerDetectClips);
                }

                _state = State.Chasing;
            }

            // Do the patrolling stuff
            if (_state == State.Patrolling)
            {

                _agent.speed = patrolSpeed;

                float distanceToCurrentWp = Vector3.Distance(transform.position, patrolPath.currentWaypoint.transform.position);

                // if we've reached the current waypoint then advance to the next one
                if (wpReachedDistanceThreshold > distanceToCurrentWp)
                {
                    patrolPath.NextWaypoint();
                    _agent.SetDestination(patrolPath.currentWaypoint.transform.position);
                }
            }

            // Do the chasing stuff
            else
            {
                _agent.speed = chaseSpeed;
                var dirToPlayer = _player.transform.position - transform.position;
                dirToPlayer.Normalize();

                // keep distance from player 
                // Yea the "dirToPlayer * 2 * _player.GetComponent<CapsuleCollider>().radius" part below is hacked in so the enemy isn't pushing the player around when trying to attack
                _agent.SetDestination(_player.transform.position - dirToPlayer * 2 * _player.GetComponent<CapsuleCollider>().radius);
  
                // IF we're in attack range then attack
                if(distanceToPlayer < attackRange)
                {
                    Attack();
                }

                // update the blind timer
                _blindChaseTimer -= Time.deltaTime;

                // Stop chasing if player is dead or we didn't see him in a while
                if (_blindChaseTimer <= 0.0f || _player.IsDead())
                {
                    _state = State.Patrolling;
                    Debug.Log("Lost player, going back to patrolling");
                    _agent.SetDestination(patrolPath.currentWaypoint.transform.position);
                }
            }
        }

        // Gracefully remove enemies that were killed
        IEnumerator Dissolve()
        {
            _rb.isKinematic = true;
            statsUI.SetActive(false);
            yield return new WaitForSeconds(2.0f);
            float distance = 0.0f;
            while (true)
            {
                var moveY = Time.deltaTime * 1;
                var pos = transform.position;
                pos.y -= moveY;
                transform.position = pos;
                distance += moveY;
                if (distance > 10.0f) // magic numbers are fun!
                    break;

                yield return null;
            }
            // yea, we could destroy it but garbage collector and stuff...
            gameObject.SetActive(false);
        }
    }

}