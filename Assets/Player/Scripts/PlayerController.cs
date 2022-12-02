using System.Collections;
using UnityEngine;
using PseudoSummon.Audio;
using System;

namespace PseudoSummon
{
    public class PlayerController : MonoBehaviour
    {
        private CapsuleCollider hitBox;
        private Animator animator;

        [SerializeField] private GameObject _playerModel;
        private Health _health;
        public Health Health { get { return _health; } }

        [Header("Util")]
        //[SerializeField] protected HealthTracker bossHealth;
        [SerializeField] private Health _bossHealth; // Temp fix for decoupling boss - switching to new Health system using event based hit dectection rather than update loop.
        [SerializeField] protected CameraHolder camHolder;

        // Serialized fields
        [Header("Player Movement")]
        [SerializeField] protected float baseMoveSpeed = 5f;
        protected float speedMod = 1f;
        public float PlayerMoveSpeed { get { return baseMoveSpeed * speedMod; } }
        [SerializeField] protected float playerRollSpeed = 8f;
        [SerializeField] protected float playerRollTime = 0.5f;
        [SerializeField] protected float rollIFrameDuration = 0.4f;
        protected bool rolling = false;

        [Header("Player Combat")]
        [SerializeField] protected GameObject dashParticle;

        // Primary
        [Header("Primary Attack")]
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected float firingInterval = 0;
        protected float fireCooldown = 0f;

        // Secondary
        [Header("Secondary Attack")]
        [SerializeField] protected GameObject busterPrefab;
        [SerializeField] protected GameObject busterChargeVFX;
        [SerializeField] protected GameObject busterFireVFX;
        [SerializeField] protected int hitsToCharge = 30; // 30
        [SerializeField] protected float windDownTime = 3f;
        protected bool busterInProgress = false;
        protected bool canStillRollOutOfBuster = true;
        protected float busterCooldown = 0;
        protected bool BusterOnCooldown { get { return busterCooldown > 0; } }
        protected int currentHitCharge = 0;
        protected void AddToCharge()
        {
            if (currentHitCharge < hitsToCharge && !BusterOnCooldown)
            {
                currentHitCharge++;
                if (currentHitCharge == hitsToCharge)
                {
                    _audio.PlaySound(playerSecondaryReadySfx);
                }
            }
        }

        public float BusterFillPercent
        {
            get
            {
                if (!BusterOnCooldown)
                {
                    return (float)(hitsToCharge - currentHitCharge) / hitsToCharge;
                }
                else
                {
                    return (float)(windDownTime - busterCooldown) / windDownTime;
                }

            }
        }

        public bool BusterCharged()
        {
            return currentHitCharge == hitsToCharge;
        }

        protected IEnumerator rollCoroutine;
        protected IEnumerator shootCoroutine;
        protected IEnumerator invincibilityCoroutine;
        protected IEnumerator hitstopCoroutine;
        protected IEnumerator HitStop(float duration)
        {
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            hitstopCoroutine = null;
            Time.timeScale = 1f;
        }

        protected Vector3 movementVector;
        protected Vector3 aimVector;

        public bool IsPaused = false;

        [Header("Player Audio")]
        [SerializeField] private SoundFile playerHitSfx;
        [SerializeField] private SoundFile playerShootSfx;
        [SerializeField] private SoundFile playerDodgeSfx;
        [SerializeField] private SoundFile playerSecondaryReadySfx;
        [SerializeField] private SoundFile playerSecondaryChargeSfx;
        [SerializeField] private SoundFile playerSecondaryFireSfx;

        // TODO: This could be moved to boss script once boss is decoupled from
        // player in future refactoring. See note on CheckBossHit method below.
        [SerializeField] private SoundFile bossHitSfx;

        private AudioProvider _audio;
        private InputProvider _input;
        private PlayerMovement _movement;

        public bool CanDie = true;
        public bool InTutorial = false;

        public Action PausePressed;
        public Action PlayerDied;

        #region UnityFunctions

        private void Awake()
        {
            hitBox = GetComponent<CapsuleCollider>();
            animator = GetComponent<Animator>();
            _audio = GetComponent<AudioProvider>();
            _input = GetComponent<InputProvider>();
            _movement = GetComponent<PlayerMovement>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _input.OnRoll += OnRoll;
            _input.OnSecondaryFire += OnBuster;
            _input.OnPause += OnPause;
            _input.OnPrimaryFireDown += OnPrimaryFireDown;
            _input.OnPrimaryFireUp += OnPrimaryFireUp;
            _health.CurrentHealthModified += OnCurrentHealthModified;
            _bossHealth.CurrentHealthModified += OnBossHit;
        }

        private void OnDisable()
        {
            _input.OnRoll -= OnRoll;
            _input.OnSecondaryFire -= OnBuster;
            _input.OnPause -= OnPause;
            _input.OnPrimaryFireDown -= OnPrimaryFireDown;
            _input.OnPrimaryFireUp -= OnPrimaryFireUp;
            _health.CurrentHealthModified -= OnCurrentHealthModified;
            _bossHealth.CurrentHealthModified -= OnBossHit;
        }

        private void Update()
        {
            //CheckBossHit();

            if (!IsPaused)
            {
                fireCooldown -= Time.deltaTime;

                HandleMovementInput();
                HandleAimingInput();
            }
        }

        private void FixedUpdate()
        {
            if (!rolling)
            {
                _movement.Move(movementVector, PlayerMoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                _movement.Move(movementVector, PlayerMoveSpeed * 0.3f * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region HitDetection

        // TODO: This could be refactored to decouple player from boss.
        private void OnBossHit(int difference)
        {
            _audio.PlaySound(bossHitSfx);
            AddToCharge();

            camHolder.CameraShake(0.1f, 0.1f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.03f);
            StartCoroutine(hitstopCoroutine);

            GameManager.Instance.AddScore(200, true);
        }

        // TODO: This could be refactored to decouple player from boss.
        // TODO: Refactor this with new Health Class - Add player attack script
        //void CheckBossHit()
        //{
        //    // Boss taking damage check
        //    if (bossHealth.TookDamage(consumeTrigger: true))
        //    {
        //        _audio.PlaySound(bossHitSfx);
        //        AddToCharge();

        //        camHolder.CameraShake(0.1f, 0.1f);

        //        if (hitstopCoroutine != null)
        //        {
        //            StopCoroutine(hitstopCoroutine);
        //        }

        //        hitstopCoroutine = HitStop(0.03f);
        //        StartCoroutine(hitstopCoroutine);

        //        GameManager.Instance.AddScore(200, true);
        //    }
        //}

        #endregion

        #region InputHandling

        private void HandleMovementInput()
        {
            movementVector = _input.GetMovementVector();
        }

        private void HandleAimingInput()
        {
            if (hitstopCoroutine != null)
            {
                return;
            }

            aimVector = _input.GetAimVector(transform.position);
            transform.LookAt(transform.position + aimVector);
        }

        #endregion

        #region HandleRoll

        private void OnRoll()
        {
            if (rollCoroutine != null || !canStillRollOutOfBuster || IsPaused)
            {
                return;
            }

            // Invincibility rolls shouldn't interrupt damage rolls
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = GoInvincible(rollIFrameDuration);
            StartCoroutine(invincibilityCoroutine);

            if (busterInProgress)
            {
                busterChargeVFX.SetActive(false);
                busterInProgress = false;
                _audio.StopSound();
            }

            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }

            Vector3 rollVector = movementVector;

            if (rollVector.magnitude <= Mathf.Epsilon)
            {
                rollVector = aimVector;
            }

            rollCoroutine = Roll(rollVector);
            StartCoroutine(rollCoroutine);
        }

        protected IEnumerator Roll(Vector3 rollDirection)
        {
            _audio.PlaySound(playerDodgeSfx);

            GameObject dashObject = Instantiate(dashParticle, transform.position, Quaternion.LookRotation(-rollDirection, Vector3.up));

            AnimateRoll(rollDirection);

            rolling = true;

            // Initial parameters
            speedMod = 1f;

            // Section of the roll in the air
            float rollInAirTimer = playerRollTime;
            while (rollInAirTimer > 0)
            {
                _movement.Move(rollDirection, playerRollSpeed * Time.fixedDeltaTime);
                rollInAirTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            rolling = false;

            speedMod = 0.3f;

            // Section of the roll on the ground
            float rollOnGroundTimer = playerRollTime * 2.0f;
            while (rollOnGroundTimer > 0)
            {
                _movement.Move(rollDirection, playerRollSpeed * rollOnGroundTimer * Time.fixedDeltaTime);
                rollOnGroundTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            Destroy(dashObject);

            rollCoroutine = null;
            if (shootCoroutine == null)
            {
                speedMod = 1f;
            }
        }

        private void AnimateRoll(Vector3 rollDirection)
        {
            float dashX = transform.InverseTransformDirection(rollDirection).normalized.x;
            float dashZ = transform.InverseTransformDirection(rollDirection).normalized.z;
            animator.SetFloat("dashX", dashX);
            animator.SetFloat("dashZ", dashZ);
            animator.Play("Dash");
        }

        #endregion

        #region HandlePause

        private void OnPause()
        {
            if (InTutorial)
            {
                return;
            }

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            PausePressed?.Invoke();
        }

        #endregion

        #region HandlePrimaryFire

        private void OnPrimaryFireDown()
        {
            StartPrimaryFire();
        }

        private void OnPrimaryFireUp()
        {
            StopPrimaryFire();
        }

        private void StartPrimaryFire()
        {
            if (InTutorial || rolling || busterInProgress || IsPaused)
            {
                return;
            }

            speedMod = 0.5f;
            shootCoroutine = ShootCoroutine();
            StartCoroutine(shootCoroutine);
        }

        private void StopPrimaryFire()
        {
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }

            speedMod = 1f;
        }

        private IEnumerator ShootCoroutine()
        {
            while (true)
            {
                if (fireCooldown <= 0 && rollCoroutine == null)
                {
                    SpawnBullet(transform.position + aimVector, aimVector);
                    AnimatePrimaryFire();

                    _audio.PlaySound(playerShootSfx);
                    fireCooldown = firingInterval;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void SpawnBullet(Vector3 origin, Vector3 direction)
        {
            TestBullet newBullet = Instantiate(bulletPrefab, origin, Quaternion.identity).GetComponent<TestBullet>();
            newBullet.SetTargetLayer(targetLayer);
            newBullet.SetDirection(direction);
        }

        private void AnimatePrimaryFire()
        {
            // Toggle Left/Right firing animation.
            animator.SetBool("FireSide", !animator.GetBool("FireSide"));

            animator.Play("Fire");
        }

        #endregion

        #region HandleBuster
        private void OnBuster()
        {
            if (InTutorial || rolling || !BusterCharged() || busterInProgress || IsPaused)
            {
                return;
            }

            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }

            shootCoroutine = Buster();
            StartCoroutine(shootCoroutine);
        }

        protected IEnumerator Buster()
        {
            // Update state
            busterInProgress = true;
            canStillRollOutOfBuster = true;
            speedMod = 0.5f;

            // Start charge effects
            busterChargeVFX.SetActive(true);
            animator.Play("BusterCharge");
            _audio.PlaySound(playerSecondaryChargeSfx);

            yield return new WaitForSeconds(2.5f);

            // Update state
            canStillRollOutOfBuster = false;

            // Create buster projectile
            GameObject busterEffect = Instantiate(busterFireVFX, transform);
            PlayerBusterAttack newBuster = Instantiate(busterPrefab, transform).GetComponent<PlayerBusterAttack>();
            newBuster.SetTargetLayer(targetLayer);

            //newBuster.SetCallback(() => Debug.Log("it worked!"));

            // Start Fire effects
            animator.Play("BusterFire");
            _audio.PlaySound(playerSecondaryFireSfx);

            // This is necessary to allow the collider a chance to register the hit
            yield return new WaitForSecondsRealtime(0.05f);

            busterEffect.transform.parent = null;
            newBuster.transform.parent = null;

            // Big hitstop and hit registration
            camHolder.CameraFlash(10f);
            camHolder.CameraShake(0.8f, 1.0f);
            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }
            hitstopCoroutine = HitStop(0.7f);
            StartCoroutine(hitstopCoroutine);

            yield return new WaitForSecondsRealtime(0.7f);

            newBuster.DamageAllCollisions();

            busterChargeVFX.SetActive(false);

            StartCoroutine(CooldownBuster());

            // Update state
            currentHitCharge = 0;
            busterInProgress = false;
            canStillRollOutOfBuster = true;
            shootCoroutine = null;
            speedMod = 1f;
        }

        protected IEnumerator CooldownBuster()
        {
            busterCooldown = windDownTime;
            while (BusterOnCooldown)
            {
                busterCooldown -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion

        private void OnCurrentHealthModified(int difference)
        {
            if (difference > 0)
            {
                return;
            }

            if (_health.CurrentHealth <= 0 && CanDie)
            {
                Die();
                return;
            }

            DamageResponse();
        }

        private void DamageResponse()
        {
            _audio.PlaySound(playerHitSfx);
                
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = GoInvincible(1f);
            StartCoroutine(invincibilityCoroutine);

            camHolder.CameraShake(0.35f, 0.35f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.3f);
            StartCoroutine(hitstopCoroutine);
        }

        protected IEnumerator GoInvincible(float duration)
        {
            hitBox.enabled = false;

            yield return new WaitForSeconds(duration);

            hitBox.enabled = true;
            invincibilityCoroutine = null;
        }

        public void Die()
        {
            StopAllCoroutines();

            _audio.StopSound();
            _audio.PlaySound(playerHitSfx);

            _playerModel.SetActive(false);
            enabled = false;

            camHolder.CameraShake(0.35f, 0.35f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.3f);
            StartCoroutine(hitstopCoroutine);

            PlayerDied?.Invoke();
        }
    }
}