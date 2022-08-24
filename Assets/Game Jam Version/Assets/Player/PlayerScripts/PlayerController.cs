using System;
using System.Collections;
using UnityEngine;

namespace PseudoSummon
{
    public class PlayerController : MonoBehaviour
    {
        // Component References
        Camera cam;
        CapsuleCollider hitBox;
        Animator animator;
        protected bool fireSide = false;
        HealthTracker health;
        [Header("Util")]
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
                    playerAudio.PlaySound(playerSecondaryReadySfx);
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

        [Header("Position Bounds")]
        [SerializeField] protected Transform platform;
        [SerializeField] protected float platformBoundsX = 9f;
        [SerializeField] protected float platformBoundsZ = 6.5f;

        protected Vector3 rawInputVector;
        protected Vector3 aimVector;

        [Header("Player Audio")]
        [SerializeField] private SoundFile playerHitSfx;
        [SerializeField] private SoundFile playerShootSfx;
        [SerializeField] private SoundFile playerDodgeSfx;
        [SerializeField] private SoundFile playerSecondaryReadySfx;
        [SerializeField] private SoundFile playerSecondaryChargeSfx;
        [SerializeField] private SoundFile playerSecondaryFireSfx;
        private IAudioPlayer playerAudio;

        public event EventHandler Died;

        public bool IsPaused = false;
        public bool CanDie = true;
        public bool IsDead = false;

        void Start()
        {
            IsPaused = false;
            cam = Camera.main;
            health = GetComponent<HealthTracker>();
            hitBox = GetComponent<CapsuleCollider>();
            animator = GetComponent<Animator>();
            playerAudio = GetComponent<IAudioPlayer>();
        }

        void CheckPlayerHit()
        {
            // Me taking damage check
            if (health.TookDamage(consumeTrigger: true))
            {
                playerAudio.PlaySound(playerHitSfx);

                if (invincibilityCoroutine != null)
                {
                    StopCoroutine(invincibilityCoroutine);
                }
                invincibilityCoroutine = GoInvincible(1f);
                StartCoroutine(invincibilityCoroutine);

                camHolder.CameraShake(0.35f, 0.35f);
                camHolder.HitStop(0.3f);
            }
        }

        // TODO: This could be refactored to decouple player from boss/other systems.
        public void OnBossHit()
        {
            if (!enabled)
            {
                return;
            }
            AddToCharge();

            camHolder.CameraShake(0.1f, 0.1f);
            camHolder.HitStop(0.03f);

            UI_ScoreTracker.Instance.AddScore(200, multiply: true);
        }

        void HandleMovementInput()
        {
            // Get Raw Axis (binary input, 0 or 1)
            Vector3 rawVec = Vector3.zero;
            rawVec.x = Input.GetAxisRaw("Horizontal");
            rawVec.z = Input.GetAxisRaw("Vertical");
            rawInputVector = rawVec;
        }

        void HandleAimingInput()
        {
            // If in middle of hit stop do not allow turning.
            if (Time.timeScale == 0f)
            {
                return;
            }

            // Get Aim vector from mouse position
            Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
            Vector3 playerToMouseVector = (Input.mousePosition - screenPos).normalized;

            aimVector = new Vector3(playerToMouseVector.x, 0, playerToMouseVector.y);
            transform.LookAt(new Vector3(transform.position.x + aimVector.x, transform.position.y, transform.position.z + aimVector.z));

            Debug.DrawRay(transform.position, aimVector * 2f, Color.blue);
        }

        void HandleFiringInput()
        {
            fireCooldown -= Time.deltaTime;

            if (UI_TutorialOnFirstPlay.Instance.FirstPlay)
            {
                return;
            }

            if (Input.GetButtonDown("Fire1") && !rolling && !busterInProgress)
            {

                shootCoroutine = Shoot();
                StartCoroutine(shootCoroutine);

            }
        }

        void HandleBusterInput()
        {
            if (UI_TutorialOnFirstPlay.Instance.FirstPlay)
            {
                return;
            }

            if (Input.GetButtonDown("Fire2") && !rolling && BusterCharged() && !busterInProgress)
            {


                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                }

                shootCoroutine = Buster();
                StartCoroutine(shootCoroutine);

            }
        }

        void HandleRollInput()
        {
            if (Input.GetButtonDown("Roll") && rollCoroutine == null && canStillRollOutOfBuster && !IsDead)
            {

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
                    playerAudio.StopSound();
                }

                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }

                // Dashy move
                rollCoroutine = Roll();
                StartCoroutine(rollCoroutine);
            }
        }

        // Update is called once per frame
        void Update()
        {

            CheckPlayerHit();

            if (!IsPaused)
            {
                HandleMovementInput();
                HandleAimingInput();
                HandleFiringInput();
                HandleBusterInput();
                HandleRollInput();
            }
        }

        protected IEnumerator Shoot()
        {

            speedMod = 0.5f;

            while (Input.GetButton("Fire1"))
            {
                if (fireCooldown <= 0 && rollCoroutine == null)
                {
                    TestBullet newBullet = Instantiate(bulletPrefab, transform.position + aimVector, Quaternion.identity).GetComponent<TestBullet>();
                    newBullet.SetTargetLayer(targetLayer);
                    newBullet.SetDirection(aimVector);

                    // Animate
                    fireSide = !fireSide;
                    animator.SetBool("FireSide", fireSide);
                    animator.Play("Fire");

                    playerAudio.PlaySound(playerShootSfx);
                    fireCooldown = firingInterval;
                }

                yield return new WaitForEndOfFrame();
            }

            shootCoroutine = null;
            speedMod = 1f;

        }

        protected IEnumerator Buster()
        {
            // 1. Begin charging.
            busterInProgress = true;
            canStillRollOutOfBuster = true;
            busterChargeVFX.SetActive(true);
            speedMod = 0.5f;

            // Animate
            animator.Play("BusterCharge");
            playerAudio.PlaySound(playerSecondaryChargeSfx);
            yield return new WaitForSeconds(2.5f);

            // 2. Create buster projectile
            canStillRollOutOfBuster = false;
            GameObject busterEffect = Instantiate(busterFireVFX, transform);
            PlayerBusterAttack newBuster = Instantiate(busterPrefab, transform).GetComponent<PlayerBusterAttack>();
            newBuster.SetTargetLayer(targetLayer);

            // Animate
            animator.Play("BusterFire");
            playerAudio.PlaySound(playerSecondaryFireSfx);
            // This is necessary to allow the collider a chance to register the hit
            yield return new WaitForSecondsRealtime(0.05f);

            busterEffect.transform.parent = null;
            newBuster.transform.parent = null;


            // 3. Big hitstop and hit registration
            camHolder.CameraFlash(10f);
            camHolder.CameraShake(0.8f, 1.0f);
            camHolder.HitStop(0.7f);

            yield return new WaitForSecondsRealtime(0.7f);

            newBuster.DamageAllCollisions();

            // Reset buster cooldown, coroutines, and start wind-down phase
            currentHitCharge = 0;

            StartCoroutine(CooldownBuster());

            busterChargeVFX.SetActive(false);
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

        protected IEnumerator GoInvincible(float duration)
        {
            hitBox.enabled = false;

            yield return new WaitForSeconds(duration);

            hitBox.enabled = true;
            invincibilityCoroutine = null;
        }

        protected IEnumerator Roll()
        {
            playerAudio.PlaySound(playerDodgeSfx);
            Vector3 rollVector;
            if (rawInputVector.magnitude > Mathf.Epsilon)
            {
                rollVector = rawInputVector;
            }
            else
            {
                rollVector = aimVector;
            }

            // Set dash direction for animation
            float dashX;
            float dashZ;

            Instantiate(dashParticle, transform.position, Quaternion.LookRotation(-rollVector, Vector3.up));

            dashX = transform.InverseTransformDirection(rollVector).normalized.x;
            dashZ = transform.InverseTransformDirection(rollVector).normalized.z;
            animator.SetFloat("dashX", dashX);
            animator.SetFloat("dashZ", dashZ);
            animator.Play("Dash");

            rolling = true;

            // Initial parameters
            speedMod = 1f;

            // Section of the roll in the air
            float rollInAirTimer = playerRollTime;
            while (rollInAirTimer > 0)
            {
                MovePlayerInBounds(rollVector, playerRollSpeed);
                rollInAirTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            rolling = false;

            speedMod = 0.3f;

            // Section of the roll on the ground
            float rollOnGroundTimer = playerRollTime * 2.0f;
            while (rollOnGroundTimer > 0)
            {
                MovePlayerInBounds(rollVector, playerRollSpeed * rollOnGroundTimer);
                rollOnGroundTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            rollCoroutine = null;
            if (shootCoroutine == null)
            {
                speedMod = 1f;
            }
        }

        void FixedUpdate()
        {
            if (health.HealthPercentage <= 0f && CanDie)
            {
                Die();
            }

            if (!rolling)
            {
                MovePlayerInBounds(rawInputVector, PlayerMoveSpeed);
            }
            else
            {
                MovePlayerInBounds(rawInputVector, PlayerMoveSpeed * 0.3f);
            }
        }

        protected void MovePlayerInBounds(Vector3 moveDirection, float moveSpeed)
        {
            if (health.HealthPercentage <= 0f)
            {
                return;
            }

            if (platform == null)
            {
                Debug.LogWarning("No platform assigned! Drag our platform object to the PlayerController script!");
                return;
            }

            // 1. Move the player
            transform.position += moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;

            // 2. Bind the player on X axis
            if (transform.position.x > platform.transform.position.x + platformBoundsX)
            {
                transform.position = new Vector3(platform.transform.position.x + platformBoundsX, transform.position.y, transform.position.z);
            }

            if (transform.position.x < platform.transform.position.x - platformBoundsX)
            {
                transform.position = new Vector3(platform.transform.position.x - platformBoundsX, transform.position.y, transform.position.z);
            }

            // 3. Bind the player on Z axis
            if (transform.position.z > platform.transform.position.z + platformBoundsZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, platform.transform.position.z + platformBoundsZ);
            }

            if (transform.position.z < platform.transform.position.z - platformBoundsZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, platform.transform.position.z - platformBoundsZ);
            }
        }

        public void Die()
        {
            Died?.Invoke(this, EventArgs.Empty);
            hitBox.enabled = false;
            enabled = false;
            gameObject.SetActive(false);
            playerAudio.StopSound();
            IsDead = true;
        }
    }
}