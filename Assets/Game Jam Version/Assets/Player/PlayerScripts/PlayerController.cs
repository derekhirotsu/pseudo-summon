using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{  
    // Component References
    Camera cam;
    CapsuleCollider hitBox;
    Animator animator;
    protected bool fireSide = false;
    HealthTracker health;
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
    protected void AddToCharge() {
        if (currentHitCharge < hitsToCharge && !BusterOnCooldown) {
            currentHitCharge++;
            if (currentHitCharge == hitsToCharge) {
                PlaySoundEffect(playerSecondaryReadySfx);
            }
        }
    }

    public float BusterFillPercent {
        get {
            if (!BusterOnCooldown) {
                return (float) (hitsToCharge - currentHitCharge) / hitsToCharge;
            } else { 
                return (float) (windDownTime - busterCooldown) / windDownTime;
            }
            
        }
    }

    public bool BusterCharged() {
        return currentHitCharge == hitsToCharge;
    }

    protected IEnumerator rollCoroutine;
    protected IEnumerator shootCoroutine;
    protected IEnumerator unpauseCoroutine;
    protected IEnumerator invincibilityCoroutine;
    protected IEnumerator hitstopCoroutine;
    protected IEnumerator HitStop(float duration) {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        hitstopCoroutine = null;
        Time.timeScale = 1f;
    }

    [Header("Position Bounds")]
    [SerializeField] protected Transform platform;
    [SerializeField] protected float platformBoundsX = 9f;
    [SerializeField] protected float platformBoundsZ = 6.5f;

    // Input Management
    [Header("Input Fields")]
    [SerializeField] protected Vector3 rawInputVector;
    public Vector3 RawInputVector { get { return rawInputVector; } }

    [SerializeField] protected Vector3 inputVector;
    public Vector3 InputVector { get { return inputVector; } }

    [SerializeField] protected Vector3 aimVector;
    public Vector3 AimVector { get { return aimVector; } }

    bool isPaused = false;

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
    private AudioSource playerAudio;

    void Start() {
        isPaused = false;
        cam = Camera.main;
        health = this.GetComponent<HealthTracker>();
        hitBox = this.GetComponent<CapsuleCollider>();
        animator = this.GetComponent<Animator>();
        playerAudio = this.GetComponent<AudioSource>();

        if (platform == null) {
            Debug.LogWarning("No Platform was given to the player!");
        }
    }

    void CheckPlayerHit() {
        // Me taking damage check
        if (health.TookDamage(consumeTrigger:true)) {
            PlaySoundEffect(playerHitSfx);

            if (invincibilityCoroutine != null ) {
                StopCoroutine(invincibilityCoroutine);
            }
            invincibilityCoroutine = GoInvincible(1f);
            StartCoroutine(invincibilityCoroutine);

            // Big hitstop
            camHolder.CameraShake(0.35f, 0.35f);

            if (hitstopCoroutine != null) {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.3f);
            StartCoroutine(hitstopCoroutine);
        }
    }

    // TODO: This could be refactored to decouple player from boss.
    void CheckBossHit() {
        // Boss taking damage check
        if (bossHealth.TookDamage(consumeTrigger:true)) {
            PlaySoundEffect(bossHitSfx);
            AddToCharge();

            // Small hitstop
            if (busterInProgress) {

            }
            camHolder.CameraShake(0.1f, 0.1f);

            if (hitstopCoroutine != null) {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = HitStop(0.03f);
            StartCoroutine(hitstopCoroutine);
            

            UI_ScoreTracker.Instance.AddScore(200, multiply:true);
        }
    }

    void HandleMovementInput() {
        // Get Raw Axis (binary input, 0 or 1)
        Vector3 rawVec = Vector3.zero;
        rawVec.x = Input.GetAxisRaw("Horizontal");
        rawVec.z = Input.GetAxisRaw("Vertical");
        rawInputVector = rawVec;

        // Get Axis (analog input, from 0 to 1)
        Vector3 axis = Vector3.zero;
        axis.x = Input.GetAxis("Horizontal");
        axis.z = Input.GetAxis("Vertical");
        inputVector = axis;
    }

    void HandleAimingInput() {
        if (hitstopCoroutine != null) {
            return;
        }

        // Get Aim vector from mouse position
        Vector3 screenPos = cam.WorldToScreenPoint(this.transform.position);
        Vector3 playerToMouseVector = (Input.mousePosition - screenPos).normalized;

        aimVector = new Vector3(playerToMouseVector.x, 0, playerToMouseVector.y);
        this.transform.LookAt(new Vector3(this.transform.position.x + aimVector.x, transform.position.y, this.transform.position.z + aimVector.z));

        Debug.DrawRay(this.transform.position, aimVector * 2f, Color.blue);
    }

    void HandleFiringInput() {
        // If Shoot and not rolling
        fireCooldown -= Time.deltaTime;

        if (UI_TutorialOnFirstPlay.Instance.FirstPlay) {
            return;
        }

        if (Input.GetButtonDown("Fire1") && !rolling && !busterInProgress) {

            shootCoroutine = Shoot();
            StartCoroutine(shootCoroutine);

        }
    }

    void HandleBusterInput() {
        // If Shoot and not rolling
        if (UI_TutorialOnFirstPlay.Instance.FirstPlay) {
            return;
        }

        if (Input.GetButtonDown("Fire2") && !rolling && BusterCharged() && !busterInProgress) {

            
            if (shootCoroutine != null) {
                StopCoroutine(shootCoroutine);
            }

            shootCoroutine = Buster();
            StartCoroutine(shootCoroutine);

        }
    }

    void HandleRollInput() {
        // If Roll
        if (Input.GetButtonDown("Roll") && rollCoroutine == null && canStillRollOutOfBuster) {

            // Invincibility rolls shouldn't interrupt damage rolls

            if (invincibilityCoroutine != null) {
                StopCoroutine(invincibilityCoroutine);
            }
            
            invincibilityCoroutine = GoInvincible(rollIFrameDuration);
            StartCoroutine(invincibilityCoroutine);

            // Debug.Log("Roll");
            if (busterInProgress) {
                busterChargeVFX.SetActive(false);
                busterInProgress = false;
                playerAudio.Stop();
            }

            if (shootCoroutine != null) {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }

            // Dashy move
            rollCoroutine = Roll();
            StartCoroutine(rollCoroutine);
            // Invincibility rolls should replace damage rolls
            // if (invincibilityCoroutine != null ) {
            //     StopCoroutine(invincibilityCoroutine);
            // }

            // invincibilityCoroutine = GoInvincible(rollIFrameDuration);
            // StartCoroutine(invincibilityCoroutine);
            
        }
    }

    void HandlePauseInput() {
        if (Input.GetButtonDown("Pause")) {
            if (UI_TutorialOnFirstPlay.Instance.FirstPlay) {
                return;
            }

            if (hitstopCoroutine != null) {
                StopCoroutine(hitstopCoroutine);
            }

            if (isPaused) {
                Unpause();
            } else {
                Music.instance.SetLowPassFilterEnabled(true);
                Debug.Log("Paused");
                Time.timeScale = 0f;
                isPaused = true;
                UI.DisplayPauseUI();
            }
        }
    }
    
    public void Unpause() {
        Music.instance.SetLowPassFilterEnabled(false);
        isPaused = false;
        Time.timeScale = 1f;
        UI.HidePauseUI();
        UI.HideOptionsUI();
    }

    // Update is called once per frame
    void Update()
    {

        CheckPlayerHit();
        CheckBossHit();

        if (!isPaused) {
            HandleMovementInput();
            HandleAimingInput();
            HandleFiringInput();
            HandleBusterInput();
            HandleRollInput();
        }

        
        HandlePauseInput();
    }

    protected IEnumerator Shoot() {

        speedMod = 0.5f;

        while (Input.GetButton("Fire1")) {

            if (fireCooldown <= 0 && rollCoroutine == null) {
                // Debug.Log("Fire!");
                TestBullet newBullet = Instantiate(bulletPrefab, this.transform.position + aimVector, Quaternion.identity).GetComponent<TestBullet>();
                newBullet.SetTargetLayer(targetLayer);
                newBullet.SetDirection(aimVector);

                // Animate
                fireSide = !fireSide;
                animator.SetBool("FireSide", fireSide);
                animator.Play("Fire");

                PlaySoundEffect(playerShootSfx);
                fireCooldown = firingInterval;
            }

            yield return new WaitForEndOfFrame();
        }

        shootCoroutine = null;
        speedMod = 1f;
        
    }

    protected IEnumerator Buster() {
        // 1. Begin charging.
        busterInProgress = true;
        canStillRollOutOfBuster = true;
        busterChargeVFX.SetActive(true);
        speedMod = 0.5f;

        // Animate
        animator.Play("BusterCharge");
        PlaySoundEffect(playerSecondaryChargeSfx);
        yield return new WaitForSeconds(2.5f);

        // 2. Create buster projectile
        canStillRollOutOfBuster = false;
        GameObject busterEffect = Instantiate(busterFireVFX, this.transform);
        PlayerBusterAttack newBuster = Instantiate(busterPrefab, this.transform).GetComponent<PlayerBusterAttack>();
        newBuster.SetTargetLayer(targetLayer);

            // Animate
        animator.Play("BusterFire");
        PlaySoundEffect(playerSecondaryFireSfx);
        // This is necessary to allow the collider a chance to register the hit
        yield return new WaitForSecondsRealtime(0.05f);

        busterEffect.transform.parent = null;
        newBuster.transform.parent = null;
        
        
        // 3. Big hitstop and hit registration
        camHolder.CameraFlash(10f);
        camHolder.CameraShake(0.8f, 1.0f);
        if (hitstopCoroutine != null) {
            StopCoroutine(hitstopCoroutine);
        }
        hitstopCoroutine = HitStop(0.7f);
        StartCoroutine(hitstopCoroutine);

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

    protected IEnumerator CooldownBuster() {
        busterCooldown = windDownTime;
        while (BusterOnCooldown) {
            busterCooldown -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    protected IEnumerator GoInvincible(float duration) {
        hitBox.enabled = false;

        yield return new WaitForSeconds(duration);

        hitBox.enabled = true;
        invincibilityCoroutine = null;
    }

    protected IEnumerator Roll() {
        PlaySoundEffect(playerDodgeSfx);
        Vector3 rollVector;
        if (rawInputVector.magnitude > Mathf.Epsilon) {
            rollVector = rawInputVector;
        } else {
            rollVector = aimVector;
        }

        // Set dash direction for animation
        float dashX;
        float dashZ;

        Instantiate(dashParticle, this.transform.position, Quaternion.LookRotation(-rollVector, Vector3.up));

        dashX = transform.InverseTransformDirection(rollVector).normalized.x;
        dashZ = transform.InverseTransformDirection(rollVector).normalized.z;
        animator.SetFloat("dashX", dashX);
        animator.SetFloat("dashZ", dashZ);
        animator.Play("Dash");
        
        rolling = true;

        // Initial parameters
        speedMod = 1f;
        float inAirTime = 0.5f;
        float onGroundTime = 1 - inAirTime;
        // hitBox.enabled = false;

        // Section of the roll in the air
        float rollInAirTimer = playerRollTime;
        while (rollInAirTimer > 0) {
            MovePlayerInBounds(rollVector, playerRollSpeed);
            rollInAirTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // hitBox.enabled = true;
        rolling = false;

        speedMod = 0.3f;

        // Section of the roll on the ground
        float rollOnGroundTimer = playerRollTime * 2.0f;
        while (rollOnGroundTimer > 0) {
            MovePlayerInBounds(rollVector, playerRollSpeed * rollOnGroundTimer);
            rollOnGroundTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        rollCoroutine = null;
        if (shootCoroutine == null) {
            speedMod = 1f;
        }
    }

    void FixedUpdate() {
        if (health.HealthPercentage <= 0f && bossHealth.HealthPercentage > 0f) {
            Die();
        }

        if (bossHealth.HealthPercentage <= 0f) {
            UI.DisplayDeathUI(won:true);
        }

        if (!rolling) {
            MovePlayerInBounds(rawInputVector, PlayerMoveSpeed);
        } else {
            MovePlayerInBounds(rawInputVector, PlayerMoveSpeed * 0.3f);
        }

        if (health.CurrentHealth == 0 || bossHealth.CurrentHealth == 0) {
            this.enabled = false;
        }
    }

    protected void MovePlayerInBounds(Vector3 moveDirection, float moveSpeed) {
        if (health.HealthPercentage <= 0f) {
            return;
        }

        // 1. Move the player
        this.transform.position += moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;

        if (platform != null) {
            // 2. Bind the player on X axis
            if (this.transform.position.x > platform.transform.position.x + platformBoundsX) {
                this.transform.position = new Vector3(platform.transform.position.x + platformBoundsX, this.transform.position.y, this.transform.position.z);
            }

            if (this.transform.position.x < platform.transform.position.x - platformBoundsX) {
                this.transform.position = new Vector3(platform.transform.position.x - platformBoundsX, this.transform.position.y, this.transform.position.z);
            }

            // 3. Bind the player on Y axis
            if (this.transform.position.z > platform.transform.position.z + platformBoundsZ) {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, platform.transform.position.z + platformBoundsZ);
            }

            if (this.transform.position.z < platform.transform.position.z - platformBoundsZ) {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, platform.transform.position.z - platformBoundsZ);
            }
        } else {
            Debug.LogWarning("No platform assigned! Drag our platform object to the PlayerController script!");
        }
        
    }

    public void Die() {
        Debug.Log("Ouchie :(");
        hitBox.enabled = false;

        playerAudio.Stop();
        UI.DisplayDeathUI(won:false);

        this.enabled = false;
        this.gameObject.SetActive(false);
    }

    // Player Audio ------------------------------------------

    bool IsAudioValid(SoundFile soundFile) {
        if (playerAudio == null || soundFile == null) {
            return false;
        }

        return true;
    }

    void SetPlayerAudioPitch(SoundFile soundFile) {
        playerAudio.pitch = 1f;
        if (soundFile.randomizePitch) {
            playerAudio.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
        }
    }

    void PlaySoundEffect(SoundFile soundFile) {
        if (!IsAudioValid(soundFile)) {
            return;
        }

        SetPlayerAudioPitch(soundFile);
        playerAudio.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
    }
}
