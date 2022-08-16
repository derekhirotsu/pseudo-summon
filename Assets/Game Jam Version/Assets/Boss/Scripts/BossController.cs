using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform boss;
    [SerializeField] protected Animator animator;
    [SerializeField] protected HealthTracker bossHealth;

    [Header("Missile Attacks")]
    [SerializeField] GameObject magicMissile;
    [SerializeField] GameObject vfx_missileWindup;
    [SerializeField] GameObject vfx_missileCastObject;

    [Header("Lightning Attacks")]
    [SerializeField] GameObject chainLightning;
    [SerializeField] GameObject vfx_lightningWindup;
    [SerializeField] GameObject vfx_lightningCastObject;

    [Header("Purple Attacks")]
    [SerializeField] GameObject lariatBurst;
    [SerializeField] GameObject vfx_lariatWindup;
    [SerializeField] GameObject vfx_lariatCast;
    [SerializeField] GameObject vfx_lariatCastObject;

    [Header("Ice Attacks")]
    [SerializeField] GameObject iceShard;
    [SerializeField] GameObject vfx_iceShardWindup;
    [SerializeField] GameObject vfx_iceShardCastObject;
    [SerializeField] GameObject vfx_iceShardCast;
 
    [Header("Cast Sequences")]
    [SerializeField] List<SpellSequence> castSequences;

    [Header("Telegraph Configs")]
    [SerializeField] protected GameObject fancyCastTrail;
    [SerializeField] protected GameObject autoWindupOrb;
    
    [Header("Config Fields")]
    [SerializeField] LayerMask bossTargets;
    // [SerializeField] float rotationSpeed;
    float rotationSpeed;
    [SerializeField] float minRotationSpeed;
    [SerializeField] float maxRotationSpeed;
    [SerializeField] float firingInterval;
    [SerializeField] bool canFire = true;

    protected bool autoPhase = false;
    bool clockwise = true;
    float yRotation = 0;
    SpellController spellController;

    float currentPhaseTime = 10;


    // Start is called before the first frame update
    void Start()
    {
        this.spellController = GetComponent<SpellController>();
        this.rotationSpeed = this.minRotationSpeed;

        StartCoroutine(ShortDelayBeforeEncounter());
    }

    protected IEnumerator ShortDelayBeforeEncounter() {
        if (UI_TutorialOnFirstPlay.Instance.FirstPlay) {
            yield return new WaitForSecondsRealtime(7.3f);
        } else {
            yield return new WaitForSeconds(2f);
        }
        
        // EnterCastingPhase();
        EnterAutoPhase();
    }

    public void EnterAutoPhase() {

        currentPhaseTime = 10f + Random.Range(2f, 5f);
        autoPhase = true;
        
        StopAllCoroutines();

        BoolTrigger("EndCast");
        fancyCastTrail.SetActive(false);

        StartCoroutine(FiringCoroutine());
        StartCoroutine(RandomizeRotationSpeed());
        StartCoroutine(RandomizeRotationDirection());
    }

    public void EnterCastingPhase() {
        // Disable all orb telegraphs
        vfx_missileCastObject.SetActive(false);
        vfx_lightningCastObject.SetActive(false);
        vfx_iceShardCastObject.SetActive(false);
        vfx_lariatCastObject.SetActive(false);

        currentPhaseTime = 15f + Random.Range(2f, 5f);
        autoPhase = false;

        StopAllCoroutines();
        StartCoroutine(BoolTrigger("ExitAuto"));
        StartCoroutine(CastingCoroutine());

    }

    protected void ChangePhase() {
        if (autoPhase) {
            // change to casting
            EnterCastingPhase();
        } else {
            //  change to auto
            EnterAutoPhase();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        currentPhaseTime -= Time.fixedDeltaTime;
        if (currentPhaseTime <= 0) {
            ChangePhase();
        }

        if ( bossHealth.HealthPercentage <= 0f) {
            Die();
        }
        
        if (autoPhase) {
            float diff = Time.deltaTime * this.rotationSpeed;
            if (clockwise) {
                this.yRotation = (this.yRotation + diff) % 360;
            } else {
                this.yRotation = (this.yRotation - diff) % 360;
            }
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, this.yRotation, 0), 0.03f);
        } else {

            // SLERP DAT QUATERNION
            this.yRotation = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,  Quaternion.Euler(0, 0, 0), 0.01f);
        }
        
    }

    public void Die() {
        StopAllCoroutines();

        this.gameObject.SetActive(false);
    }

    private IEnumerator RandomizeRotationSpeed() {
        while (true) {
            float test = Random.Range(this.minRotationSpeed, this.maxRotationSpeed);
            float test0 = Mathf.Floor(test);

            this.rotationSpeed = test0;

            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator RandomizeRotationDirection() {
        while (true) {
            this.clockwise = Random.Range(0f, 1f) < 0.5f;

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator FiringCoroutine() {
        while (this.canFire) {

            animator.Play("AutoWindup");

            float test = Random.Range(0f, 1f);
            int result = Random.Range(0, 4);
            // int result = 3;

            switch(result) {
                case 0: 
                    // yield return StartCoroutine(MissileBarrage());
                    AudioManager.instance.PlayOneShotSoundFile("boss ice wave charge");
                    Instantiate(vfx_missileWindup, autoWindupOrb.transform);
                    break;

                case 1:
                    // yield return StartCoroutine(ChainLightning());
                    AudioManager.instance.PlayOneShotSoundFile("boss ice wave charge");
                    Instantiate(vfx_lightningWindup, autoWindupOrb.transform);
                    break;
                
                case 2:
                    // yield return StartCoroutine(LariatBurst());
                    AudioManager.instance.PlayOneShotSoundFile("boss ice wave charge");
                    Instantiate(vfx_lariatWindup, autoWindupOrb.transform);
                    break;

                case 3:
                    // yield return StartCoroutine(IceWaveVolley());
                    AudioManager.instance.PlayOneShotSoundFile("boss ice wave charge");
                    Instantiate(vfx_iceShardWindup, autoWindupOrb.transform);
                    break;

            }

            yield return new WaitForSeconds(0.5f);

            switch(result) {
                case 0:
                    yield return StartCoroutine(MissileBarrage());
                    break;

                case 1:
                    yield return StartCoroutine(ChainLightning());
                    break;
                
                case 2:
                    yield return StartCoroutine(LariatBurst());
                    break;

                case 3:
                    yield return StartCoroutine(IceWaveVolley());
                    break;

            }

            // Fire();
            // yield return new WaitForSeconds(this.firingInterval);
        }
    }

    protected IEnumerator BoolTrigger(string name) {
        animator.SetBool(name, true);

        yield return new WaitForSeconds(0.1f);

        animator.SetBool(name, false);
    }

    private void Fire(GameObject projectile, Vector3 positionOffset, Vector3 rotationOffset, float deviation = 0) {
        Vector3 aimVector = (this.player.position - this.boss.position).normalized;
        aimVector.y = 0;
        Vector3 spawnLocation = ( this.boss.position + positionOffset) + (aimVector * 1);

        TestBullet newBullet = Instantiate(projectile, spawnLocation, Quaternion.identity).GetComponent<TestBullet>();
        // newBullet.transform.rotation = Quaternion.FromToRotation(newBullet.transform.rotation, Quaternion.Euler())
        newBullet.SetTargetLayer(bossTargets);

        // Random deviation
        aimVector.x += Random.Range(-deviation, deviation);
        aimVector.z += Random.Range(-deviation, deviation);
        aimVector = aimVector.normalized;

        newBullet.SetDirection(aimVector);
    }

    private IEnumerator IceWaveVolley() {
        vfx_iceShardCastObject.SetActive(true);
        for (int i = 0; i < 4; i += 1) {
            AudioManager.instance.PlayOneShotSoundFile("boss ice wave fire");
            Instantiate(vfx_iceShardCast, autoWindupOrb.transform);
            for (int j = 0; j < 40; j += 1) {
                Fire(iceShard, Vector3.zero, Vector3.zero, 0.8f);
            }
            yield return new WaitForSeconds(1f);
        }

        vfx_iceShardCastObject.SetActive(false);
        StartCoroutine(BoolTrigger("ExitAuto"));
        yield return new WaitForSeconds(this.firingInterval);
    }

    private IEnumerator MissileBarrage() {
        vfx_missileCastObject.SetActive(true);
        for (int i = 0; i < 60; i += 1) {
            Fire(magicMissile, Vector3.zero, Vector3.zero, 0.8f);
            AudioManager.instance.PlayOneShotSoundFile("boss missile barrage fire");
            yield return new WaitForSeconds(0.08f);
        }
        StartCoroutine(BoolTrigger("ExitAuto"));
        vfx_missileCastObject.SetActive(false);
        yield return new WaitForSeconds(this.firingInterval);
    }

    private IEnumerator LariatBurst() {
        vfx_lariatCastObject.SetActive(true);
        for (int i = 0; i < 5; i += 1) {
            Instantiate(vfx_lariatCast, autoWindupOrb.transform);
            Fire(lariatBurst, Vector3.zero, Vector3.zero);
            AudioManager.instance.PlayOneShotSoundFile("boss lariat fire");
            yield return new WaitForSeconds(1f);
        }
        vfx_lariatCastObject.SetActive(false);
        StartCoroutine(BoolTrigger("ExitAuto"));
        yield return new WaitForSeconds(this.firingInterval);
    }

    private IEnumerator ChainLightning() {
        vfx_lightningCastObject.SetActive(true);
        for (int i = 0; i < 200; i += 1) {
            Fire(chainLightning, new Vector3(3, 1, 0), Vector3.zero);
            AudioManager.instance.PlayOneShotSoundFile("boss lightning fire");
            yield return new WaitForSeconds(0.05f);
        }
        vfx_lightningCastObject.SetActive(false);
        StartCoroutine(BoolTrigger("ExitAuto"));
        yield return new WaitForSeconds(this.firingInterval);
    }

    private IEnumerator CastingCoroutine() {
        // Wait some time for him to reach the top
        yield return new WaitForSeconds(3f);

        animator.Play("Casting");
        fancyCastTrail.SetActive(true);

        // Get a random sequence
        int sequenceIndex = Random.Range(0, castSequences.Count);
        
        SpellSequence sequence = castSequences[sequenceIndex];
            for (int tally = 0; tally < sequence.SpellID.Count; tally++) {

            if (sequence.SpellID[tally] < 100) {
                // Cast FireBall Volley
                this.spellController.CastFireball(sequence.SpellID[tally]);

            } else if ( sequence.SpellID[tally] >= 100 && sequence.SpellID[tally] < 200) {
                // Cast Fire Field
                this.spellController.CastFireField(player.position.x, player.position.z);
            }

            
            yield return new WaitForSeconds(sequence.Delays[tally]);

        }

        ChangePhase();
    }
}
