using System.Collections;
using UnityEngine;
using PseudoSummon.Audio;
using System;

namespace PseudoSummon
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Util")]
        [SerializeField] protected CameraHolder camHolder;
        [SerializeField] private PlayerStats _stats;

        [Header("Player Visuals")]
        [SerializeField] private GameObject _playerModel;
        [SerializeField] protected GameObject dashParticle;

        [Header("Primary Attack")]
        [SerializeField] private PrimaryAttackProjectile _primaryAttack;
        protected float fireCooldown = 0f;

        [Header("Secondary Attack")]
        [SerializeField] protected GameObject busterPrefab;
        [SerializeField] protected GameObject busterChargeVFX;
        [SerializeField] protected GameObject busterFireVFX;

        [Header("Player Audio")]
        [SerializeField] private SoundFile playerHitSfx;
        [SerializeField] private SoundFile playerShootSfx;
        [SerializeField] private SoundFile playerDodgeSfx;
        [SerializeField] private SoundFile playerSecondaryReadySfx;
        [SerializeField] private SoundFile playerSecondaryChargeSfx;
        [SerializeField] private SoundFile playerSecondaryFireSfx;

        protected bool BusterOnCooldown { get { return busterCooldown > 0; } }
        protected void AddToCharge()
        {
            if (currentHitCharge < _stats.BusterHitsToCharge && !BusterOnCooldown)
            {
                currentHitCharge++;
                if (currentHitCharge == _stats.BusterHitsToCharge)
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
                    return (float)(_stats.BusterHitsToCharge - currentHitCharge) / _stats.BusterHitsToCharge;
                }

                return (float)(_stats.BusterCooldownTime - busterCooldown) / _stats.BusterCooldownTime;
            }
        }

        public bool BusterCharged { get { return currentHitCharge == _stats.BusterHitsToCharge; } }

        protected IEnumerator rollCoroutine;
        protected IEnumerator shootCoroutine;
        protected IEnumerator invincibilityCoroutine;
        protected IEnumerator hitstopCoroutine;
        protected IEnumerator HitStop(float duration)
        {
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            hitstopCoroutine = null;
        }

        private CapsuleCollider hitBox;
        private Animator animator;
        private AudioProvider _audio;
        private InputProvider _input;
        private PlayerMovement _movement;
        private Health _health;
        public Health Health { get { return _health; } }

        public bool IsPaused = false;
        public bool CanDie = true;
        public bool InTutorial = false;

        private bool _isRolling = false;
        private bool _isBusterInProgress = false;
        private bool _canStillRollOutOfBuster = true;

        private float speedModifier = 1f;
        public float PlayerMoveSpeed { get { return _stats.BaseMoveSpeed * speedModifier; } }

        private float busterCooldown = 0;
        private int currentHitCharge = 0;

        private Vector3 movementVector;
        private Vector3 aimVector;

        public Action PausePressed;
        public Action PlayerDied;
        public Action<int, bool> ScoreModified;

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
        }

        private void OnDisable()
        {
            _input.OnRoll -= OnRoll;
            _input.OnSecondaryFire -= OnBuster;
            _input.OnPause -= OnPause;
            _input.OnPrimaryFireDown -= OnPrimaryFireDown;
            _input.OnPrimaryFireUp -= OnPrimaryFireUp;
            _health.CurrentHealthModified -= OnCurrentHealthModified;
        }

        private void Update()
        {
            if (!IsPaused)
            {
                fireCooldown -= Time.deltaTime;

                HandleMovementInput();
                HandleAimingInput();
            }

            float moveSpeed = _isRolling ? PlayerMoveSpeed * 0.3f * Time.deltaTime : PlayerMoveSpeed * Time.deltaTime;
            _movement.Move(movementVector, moveSpeed);
        }

        #endregion

        #region HitDetection

        public void OnBossHit()
        {
            AddToCharge();
            ScoreModified?.Invoke(200, true);

            camHolder.CameraShake(0.1f, 0.1f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.03f);
            StartCoroutine(hitstopCoroutine);
        }

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
            hitBox.enabled = false;

            camHolder.CameraShake(0.35f, 0.35f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.3f);
            StartCoroutine(hitstopCoroutine);

            PlayerDied?.Invoke();
        }

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
            if (rollCoroutine != null || !_canStillRollOutOfBuster || IsPaused)
            {
                return;
            }

            // Invincibility rolls shouldn't interrupt damage rolls
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = GoInvincible(_stats.RollInvincibilityDuration);
            StartCoroutine(invincibilityCoroutine);

            if (_isBusterInProgress)
            {
                busterChargeVFX.SetActive(false);
                _isBusterInProgress = false;
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

            _isRolling = true;

            // Initial parameters
            speedModifier = 1f;

            // Section of the roll in the air
            float rollInAirTimer = _stats.RollDuration;
            while (rollInAirTimer > 0)
            {
                _movement.Move(rollDirection, _stats.RollSpeed * Time.fixedDeltaTime);
                rollInAirTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            _isRolling = false;

            speedModifier = 0.3f;

            // Section of the roll on the ground
            float rollOnGroundTimer = _stats.RollDuration * 2.0f;
            while (rollOnGroundTimer > 0)
            {
                _movement.Move(rollDirection, _stats.RollSpeed * rollOnGroundTimer * Time.fixedDeltaTime);
                rollOnGroundTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            Destroy(dashObject);

            rollCoroutine = null;
            if (shootCoroutine == null)
            {
                speedModifier = 1f;
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
            if (InTutorial || _isRolling || _isBusterInProgress || IsPaused)
            {
                return;
            }

            speedModifier = 0.5f;
            shootCoroutine = ShootCoroutine();
            StartCoroutine(shootCoroutine);
        }

        private void StopPrimaryFire()
        {
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }

            speedModifier = 1f;
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
                    fireCooldown = _stats.PrimaryFireAttackInterval;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void SpawnBullet(Vector3 origin, Vector3 direction)
        {
            PrimaryAttackProjectile newProjectile = Instantiate(_primaryAttack, origin, Quaternion.identity);
            newProjectile.SetPlayerController(this);
            newProjectile.SetDirection(direction);
            newProjectile.CollisionMask = _stats.AttackCollisionMask;
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
            if (InTutorial || _isRolling || !BusterCharged || _isBusterInProgress || IsPaused)
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
            BusterAttackChargeUp();

            yield return new WaitForSeconds(2.5f);

            // Update state
            _canStillRollOutOfBuster = false;

            // Create buster projectile
            GameObject busterEffect = Instantiate(busterFireVFX, transform); // why use transform to parent this ...
            PlayerBusterAttack newBuster = Instantiate(busterPrefab, transform).GetComponent<PlayerBusterAttack>();
            newBuster.SetTargetLayer(_stats.AttackCollisionMask);

            //newBuster.SetCallback(() => Debug.Log("it worked!"));

            // Start Fire effects
            animator.Play("BusterFire");
            _audio.PlaySound(playerSecondaryFireSfx);

            // This is necessary to allow the collider a chance to register the hit
            yield return new WaitForSecondsRealtime(0.05f);

            busterEffect.transform.parent = null; // ... when it gets unparented here? just set origin and rotation originally. Its only locally referenced here to unparent it. Could this be part of the attack script?
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
            _isBusterInProgress = false;
            _canStillRollOutOfBuster = true;
            shootCoroutine = null;
            speedModifier = 1f;
        }

        private void BusterAttackChargeUp()
        {
            // Update state
            _isBusterInProgress = true;
            _canStillRollOutOfBuster = true;
            speedModifier = 0.5f;

            // Start charge effects
            busterChargeVFX.SetActive(true);
            animator.Play("BusterCharge");
            _audio.PlaySound(playerSecondaryChargeSfx);
        }

        protected IEnumerator CooldownBuster()
        {
            busterCooldown = _stats.BusterCooldownTime;
            while (BusterOnCooldown)
            {
                busterCooldown -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion
    }
}