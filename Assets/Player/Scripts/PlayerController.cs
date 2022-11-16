using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PseudoSummon.Audio;

public class PlayerController : MonoBehaviour
{
    private CapsuleCollider hitBox;
    private Animator animator;
    private HealthTracker health;

    [Header("Util")]
    [SerializeField] protected PlayerCanvas UI;
    [SerializeField] protected HealthTracker bossHealth;
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
    protected IEnumerator unpauseCoroutine;
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

    private bool isPaused = false;

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

    #region UnityFunctions

    private void Awake()
    {
        health = GetComponent<HealthTracker>();
        hitBox = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        _audio = GetComponent<AudioProvider>();
        _input = GetComponent<InputProvider>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        _input.OnRoll += OnRoll;
        _input.OnSecondaryFire += OnBuster;
        _input.OnPause += OnPause;
        _input.OnPrimaryFireDown += OnPrimaryFireDown;
        _input.OnPrimaryFireUp += OnPrimaryFireUp;
    }

    private void OnDisable()
    {
        _input.OnRoll -= OnRoll;
        _input.OnSecondaryFire -= OnBuster;
        _input.OnPause -= OnPause;
        _input.OnPrimaryFireDown += OnPrimaryFireDown;
        _input.OnPrimaryFireUp += OnPrimaryFireUp;
    }

    private void Update()
    {
        CheckPlayerHit();
        CheckBossHit();

        if (!isPaused)
        {
            fireCooldown -= Time.deltaTime;

            HandleMovementInput();
            HandleAimingInput();
        }
    }

    private void FixedUpdate()
    {
        if (health.HealthPercentage <= 0f && bossHealth.HealthPercentage > 0f)
        {
            Die();
            return;
        }

        if (bossHealth.HealthPercentage <= 0f)
        {
            UI.DisplayDeathUI(won: true);
        }

        if (!rolling)
        {
            _movement.Move(movementVector, PlayerMoveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            _movement.Move(movementVector, PlayerMoveSpeed * 0.3f * Time.fixedDeltaTime);
        }

        if (bossHealth.CurrentHealth == 0)
        {
            enabled = false;
        }
    }

    #endregion

    #region HitDetection
    void CheckPlayerHit()
    {
        // Me taking damage check
        if (health.TookDamage(consumeTrigger: true))
        {
            _audio.PlaySound(playerHitSfx);

            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }
            invincibilityCoroutine = GoInvincible(1f);
            StartCoroutine(invincibilityCoroutine);

            // Big hitstop
            camHolder.CameraShake(0.35f, 0.35f);

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.3f);
            StartCoroutine(hitstopCoroutine);
        }
    }

    // TODO: This could be refactored to decouple player from boss.
    void CheckBossHit()
    {
        // Boss taking damage check
        if (bossHealth.TookDamage(consumeTrigger: true))
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


            UI_ScoreTracker.Instance.AddScore(200, multiply: true);
        }
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
        if (rollCoroutine != null || !canStillRollOutOfBuster || isPaused)
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
        if (UI_TutorialOnFirstPlay.Instance.FirstPlay)
        {
            return;
        }

        if (hitstopCoroutine != null)
        {
            StopCoroutine(hitstopCoroutine);
        }

        if (isPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        Music.Instance.SetLowPassFilterEnabled(true);
        Time.timeScale = 0f;
        isPaused = true;
        UI.DisplayPauseUI();
    }

    public void Unpause()
    {
        Music.Instance.SetLowPassFilterEnabled(false);
        isPaused = false;
        Time.timeScale = 1f;
        UI.HidePauseUI();
        UI.HideOptionsUI();
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

    private void StartPrimaryFire() {
        if (UI_TutorialOnFirstPlay.Instance.FirstPlay || rolling || busterInProgress || isPaused)
        {
            return;
        }

        speedMod = 0.5f;
        shootCoroutine = ShootCoroutine();
        StartCoroutine(shootCoroutine);
    }

    private void StopPrimaryFire()
    {
        if (shootCoroutine != null )
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
        if (UI_TutorialOnFirstPlay.Instance.FirstPlay || rolling || !BusterCharged() || busterInProgress || isPaused)
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

    protected IEnumerator GoInvincible(float duration)
    {
        hitBox.enabled = false;

        yield return new WaitForSeconds(duration);

        hitBox.enabled = true;
        invincibilityCoroutine = null;
    }

    public void Die()
    {
        hitBox.enabled = false;

        _audio.StopSound();
        UI.DisplayDeathUI(won: false);

        enabled = false;
        gameObject.SetActive(false);
    }
}
