using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Base unit for player and enemies
    /// </summary>
    public class BaseUnit : MonoBehaviour
    {
        // max health unit can have
        public int maxHealth;
        // health the unit starts with
        public int startHealth;
        // max energy the unit can have
        public int maxEnergy;
        // energy the unit starts with
        public int startEnergy;

        // cost of a single attack in energy
        public int attackEnergyCost;
        // cooldown between attacks
        public float attackWaitTime = 0.2f;

        // reference to the stats UI canvas
        public GameObject statsUI;

        // damage a single hit causes
        public int damagePerHit;

        // hp bar UI ref
        public StatBar hpBar;
        // energy bar UI ref
        public StatBar energyBar;

        // Energy regeneration interval time
        public float energyRegenInterval;
        // amount of energy regenerated per inteval
        public int energyRegenRate;

        #region public properties
        public int health { get { return _health; } }
        public int energy { get { return _energy; } }
        #endregion

        private int _health;
        private int _energy;
        private float _energyRegenTimer;
        private float _attackWaitTimer;
        private bool _canAttack = true;

        // Audio clips
        public AudioClip[] footstepClips;
        public AudioClip[] swordHitClips;
        public AudioClip[] swordSwingClips;
        public AudioClip[] deathClips;

        // flags if left or right foot are currently down
        private bool _footDownL;
        private bool _footDownR;

        protected Rigidbody _rb;
        protected Animator _animator;

        // ref to this unit's enemy detector component
        public EnemyDetector enemyDetector;

        protected virtual void Update()
        {
            // update the attack cooldown
            if(!_canAttack)
            {
                _attackWaitTimer -= Time.deltaTime;
                if (_attackWaitTimer <= 0.0f)
                    _canAttack = true;
            }
        }

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            _health = Mathf.Min(startHealth, maxHealth);
            _energy = startEnergy;
            _energyRegenTimer = 0;

            // start energy regen coroutine
            StartCoroutine(UpdateCurrentEnergy());
            UpdateBars();
        }

        // footstep sounds are done in here. 
        void OnAnimatorMove()
        {
            if (_animator.GetFloat("footDownL") > 0.5f)
            {
                if (!_footDownL)
                    PlayFootestepSound();
                

                _footDownL = true;
            }
            else
            {
                _footDownL = false;
            }


            if (_animator.GetFloat("footDownR") > 0.5f)
            {
                if (!_footDownR)
                    PlayFootestepSound();
                

                _footDownR = true;
            }
            else
            {
                _footDownR = false;
            }

            //Debug.Log(_animator.GetFloat("footDown") + " " + _animator.GetFloat("test"));
        }

        // Is dead getter
        public bool IsDead()
        {
            return _health <= 0;
        }

        // Update current energy every energyRegenInterval
        private IEnumerator UpdateCurrentEnergy()
        {
            yield return new WaitForSeconds(energyRegenInterval);
            _energy += energyRegenRate;
            _energy = Mathf.Min(maxEnergy, _energy);

            UpdateBars();
            // restart coroutine
            StartCoroutine(UpdateCurrentEnergy());
        }

        // Damage this unit
        protected virtual void TakeDamage(int amount)
        {
            if (IsDead())
                return;

            Debug.Log("Taking damage: " + amount);

            _health -= amount;
            _health = Mathf.Max(_health, 0);

            UpdateBars();

            // Call kill if this killed the unit
            if (_health <= 0)
                Kill();
        }

        public virtual void Kill()
        {
            Debug.Log("I am kill");     
            // play death sound       
            PlayRandomClip(deathClips);
        }

        private void UpdateBars()
        {
            hpBar.SetStat(health, maxHealth);
            energyBar.SetStat(energy, maxEnergy);
        }

        // Try to attack
        protected void Attack()
        {
            // Still on cooldown, nope
            if (!_canAttack)
                return;

            // Not enough energy, NOPE
            if (energy <= attackEnergyCost)
                return;

            // Already on the attack animation... NOPE (bit redundant with the cooldown I know)
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            // sword woosh sound
            PlaySwordSwingSound();

            // play attack animation
            _animator.SetTrigger("attack");
            SpendEnergy(attackEnergyCost);

            _attackWaitTimer = attackWaitTime;
            _canAttack = false;
        }

        // Spend energy
        protected void SpendEnergy(int amount)
        {
            _energy -= amount;
            _energy = Mathf.Max(_energy, 0);

            UpdateBars();

        }

        // Plays a random one shot clip
        protected void PlayRandomClip(AudioClip[] clips)
        {
            if (clips.Length < 1)
                return;

            var clipIndex = Random.Range(0, clips.Length);
            GetComponent<AudioSource>().PlayOneShot(clips[clipIndex]);
        }
        
        public void PlayFootestepSound()
        {
            PlayRandomClip(footstepClips);
        }

        public void PlaySwordSwingSound()
        {
            PlayRandomClip(swordSwingClips);
        }

        public void PlaySwordHitSound()
        {
            PlayRandomClip(swordHitClips);
        }

        // Actually attack and damage anything in range
        public virtual void DoAttack()
        {
            foreach (var enemy in enemyDetector.enemyCache)
            {
                if (enemy == null)
                    return;

                PlaySwordHitSound();
                Debug.Log("Attacking enemy " + enemy.name);
                enemy.TakeDamage(damagePerHit);
            }
        }

    }

}