using PseudoSummon.Audio;
using PseudoSummon.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform boss;
        [SerializeField] protected Animator animator;

        private Health _health;
        public Health Health { get { return _health; } }

        [Header("Magic Missile Barrage Attack")]
        [SerializeField] private Projectile _magicMissile;
        [SerializeField] private GameObject vfx_missileWindup;
        [SerializeField] private GameObject vfx_missileCastObject;
        [SerializeField, Min(1)] private int _numberOfMissilesPerAttack = 60;
        [SerializeField, Min(0)] private float _magicMissileDegreeDeviation = 60f;
        [SerializeField, Min(0)] private float _secondsBetweenMissiles = 0.08f;

        [Header("Chain Lightning Attack")]
        [SerializeField] private Projectile _chainLightning;
        [SerializeField] private GameObject vfx_lightningWindup;
        [SerializeField] private GameObject vfx_lightningCastObject;
        [SerializeField, Min(1)] private int _chainLightningProjectilesPerAttack = 200;
        [SerializeField, Min(0)] private float _secondsBetweenChainLightning = 0.05f;

        [Header("Lariat Attack")]
        [SerializeField] private Projectile _lariat;
        [SerializeField] private GameObject vfx_lariatWindup;
        [SerializeField] private GameObject vfx_lariatCast;
        [SerializeField] private GameObject vfx_lariatCastObject;
        [SerializeField, Min(0)] private float _secondsBetweenLariats = 1f;
        [SerializeField, Min(1)] private int _numberOfLariatsPerAttack = 5;

        [Header("Ice Wave Volley Attack")]
        [SerializeField] private Projectile _iceWave;
        [SerializeField] private GameObject vfx_iceShardWindup;
        [SerializeField] private GameObject vfx_iceShardCastObject;
        [SerializeField] private GameObject vfx_iceShardCast;
        [SerializeField, Min(1)] private int _numberOfIceWaveVolleys = 4;
        [SerializeField, Min(1)] private int _iceShardsPerVolley = 40;
        [SerializeField, Min(0)] private float _iceWaveDegreeDeviation = 60f;
        [SerializeField, Min(0)] private float _secondsBetweenIceWaveVolleys = 1f;

        [Header("Cast Sequences")]
        [SerializeField] private List<SpellSequence> castSequences;

        [Header("Telegraph Configs")]
        [SerializeField] protected GameObject fancyCastTrail;
        [SerializeField] protected GameObject autoWindupOrb;

        [Header("Config Fields")]
        [SerializeField] LayerMask bossTargets;
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [SerializeField] private float firingInterval;
        [SerializeField] private bool canFire = true;
        [SerializeField, Min(0)] private float _minAutoPhaseTimeSeconds = 12f;
        [SerializeField, Min(0)] private float _maxAutoPhaseTimeSeconds = 15f;
        [SerializeField, Min(0)] private float _minCastingPhaseTimeSeconds = 17f;
        [SerializeField, Min(0)] private float _maxCastingPhaseTimeSeconds = 20f;
        [SerializeField, Min(0)] private float _minDirectionChangeTimeSeconds = 1f;
        [SerializeField, Min(0)] private float _maxDirectionChangeTimeSeconds = 3f;

        [Header("Boss Audio")]
        [SerializeField] private SoundFile autoAttackChargeSfx;
        [SerializeField] private SoundFile iceWaveFireSfx;
        [SerializeField] private SoundFile missileBarrageFireSfx;
        [SerializeField] private SoundFile lariatFireSfx;
        [SerializeField] private SoundFile lightningFireSfx;

        private float rotationSpeed;
        protected bool autoPhase = false;
        private bool clockwise = true;
        private float yRotation = 0;
        private float currentPhaseTime = 10;

        private SpellController spellController;
        private AudioProvider _audio;

        private WaitForSeconds _magicMissileBarrageInterval;
        private WaitForSeconds _lightningInterval;
        private WaitForSeconds _lariatInterval;
        private WaitForSeconds _iceWaveVolleyInterval;

        public System.Action BossDied;

        #region UnityFunctions

        private void Awake()
        {
            spellController = GetComponent<SpellController>();
            _audio = GetComponent<AudioProvider>();
            _health = GetComponentInChildren<Health>();
        }

        private void OnEnable()
        {
            _health.CurrentHealthModified += OnCurrentHealthModified;
        }

        private void OnDisable()
        {
            _health.CurrentHealthModified -= OnCurrentHealthModified;
        }

        private void Start()
        {
            _magicMissileBarrageInterval = new WaitForSeconds(_secondsBetweenMissiles);
            _lightningInterval = new WaitForSeconds(_secondsBetweenChainLightning);
            _lariatInterval = new WaitForSeconds(_secondsBetweenLariats);
            _iceWaveVolleyInterval = new WaitForSeconds(_secondsBetweenIceWaveVolleys);
            rotationSpeed = minRotationSpeed;
            StartCoroutine(ShortDelayBeforeEncounter());
        }

        private void FixedUpdate()
        {
            currentPhaseTime -= Time.fixedDeltaTime;
            if (currentPhaseTime <= 0)
            {
                ChangePhase();
            }

            if (autoPhase)
            {
                float diff = clockwise ? Time.deltaTime * rotationSpeed : -(Time.deltaTime * rotationSpeed);
                yRotation = (yRotation + diff) % 360;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yRotation, 0), 0.03f);
            }
            else
            {
                yRotation = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.01f);
            }
        }

        #endregion

        protected IEnumerator ShortDelayBeforeEncounter()
        {
            if (UI_TutorialOnFirstPlay.Instance.FirstPlay)
            {
                yield return new WaitForSecondsRealtime(7.3f);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            EnterAutoPhase();
        }

        public void EnterAutoPhase()
        {
            currentPhaseTime = Random.Range(_minAutoPhaseTimeSeconds, _maxAutoPhaseTimeSeconds);
            autoPhase = true;

            StopAllCoroutines();

            BoolTrigger("EndCast");
            fancyCastTrail.SetActive(false);

            StartCoroutine(FiringCoroutine());
            StartCoroutine(RandomizeRotationSpeed());
            StartCoroutine(RandomizeRotationDirection());
        }

        public void EnterCastingPhase()
        {
            // Disable all orb telegraphs
            vfx_missileCastObject.SetActive(false);
            vfx_lightningCastObject.SetActive(false);
            vfx_iceShardCastObject.SetActive(false);
            vfx_lariatCastObject.SetActive(false);

            currentPhaseTime = Random.Range(_minCastingPhaseTimeSeconds, _maxCastingPhaseTimeSeconds);
            autoPhase = false;

            StopAllCoroutines();
            StartCoroutine(BoolTrigger("ExitAuto"));
            StartCoroutine(CastingCoroutine());

        }

        protected void ChangePhase()
        {
            if (autoPhase)
            {
                EnterCastingPhase();
            }
            else
            {
                EnterAutoPhase();
            }
        }

        private IEnumerator RandomizeRotationSpeed()
        {
            WaitForSeconds speedChangeWait = new WaitForSeconds(4f);

            while (true)
            {
                float nextRotationSpeed = Mathf.Floor(Random.Range(minRotationSpeed, maxRotationSpeed));

                rotationSpeed = nextRotationSpeed;

                yield return speedChangeWait;
            }
        }

        private IEnumerator RandomizeRotationDirection()
        {
            while (true)
            {
                clockwise = Random.value < 0.5f;
                float waitTime = Random.Range(_minDirectionChangeTimeSeconds, _maxDirectionChangeTimeSeconds);

                yield return new WaitForSeconds(waitTime);
            }
        }

        private IEnumerator FiringCoroutine()
        {
            WaitForSeconds windupDelay = new WaitForSeconds(0.5f);

            while (canFire)
            {
                animator.Play("AutoWindup");

                int result = Random.Range(0, 4);

                switch (result)
                {
                    case 0:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_missileWindup, autoWindupOrb.transform);
                        yield return windupDelay;
                        yield return StartCoroutine(MissileBarrage());
                        break;

                    case 1:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_lightningWindup, autoWindupOrb.transform);
                        yield return windupDelay;
                        yield return StartCoroutine(ChainLightning());
                        break;

                    case 2:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_lariatWindup, autoWindupOrb.transform);
                        yield return windupDelay;
                        yield return StartCoroutine(LariatBurst());
                        break;

                    case 3:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_iceShardWindup, autoWindupOrb.transform);
                        yield return windupDelay;
                        yield return StartCoroutine(IceWaveVolley());
                        break;
                }

                StartCoroutine(BoolTrigger("ExitAuto"));
                yield return new WaitForSeconds(firingInterval);
            }
        }

        protected IEnumerator BoolTrigger(string name)
        {
            animator.SetBool(name, true);

            yield return new WaitForSeconds(0.1f);

            animator.SetBool(name, false);
        }

        private void FireProjectile(Projectile projectile, float degreeDeviation = 0)
        {
            Vector3 aimVector = (player.position - boss.position).normalized;
            aimVector.y = 0;
            Vector3 spawnLocation = boss.position + aimVector * 1;

            aimVector = Quaternion.Euler(0, Random.Range(-degreeDeviation, degreeDeviation), 0) * aimVector;

            Projectile newProjectile = Instantiate(projectile, spawnLocation, Quaternion.identity).GetComponent<Projectile>();
            newProjectile.SetDirection(aimVector);
        }

        private IEnumerator IceWaveVolley()
        {
            vfx_iceShardCastObject.SetActive(true);
            for (int i = 0; i < _numberOfIceWaveVolleys; i++)
            {
                _audio.PlaySound(iceWaveFireSfx);
                Instantiate(vfx_iceShardCast, autoWindupOrb.transform);
                for (int j = 0; j < _iceShardsPerVolley; j++)
                {
                    FireProjectile(_iceWave, _iceWaveDegreeDeviation);
                }
                yield return _iceWaveVolleyInterval;
            }
            vfx_iceShardCastObject.SetActive(false);
        }
        
        private IEnumerator MissileBarrage()
        {
            vfx_missileCastObject.SetActive(true);
            for (int i = 0; i < _numberOfMissilesPerAttack; i++)
            {
                FireProjectile(_magicMissile, _magicMissileDegreeDeviation);
                _audio.PlaySound(missileBarrageFireSfx);
                yield return _magicMissileBarrageInterval;
            }
            vfx_missileCastObject.SetActive(false);
        }

        private IEnumerator LariatBurst()
        {
            vfx_lariatCastObject.SetActive(true);
            for (int i = 0; i < _numberOfLariatsPerAttack; i++)
            {
                Instantiate(vfx_lariatCast, autoWindupOrb.transform);
                FireProjectile(_lariat);
                _audio.PlaySound(lariatFireSfx);
                yield return _lariatInterval;
            }
            vfx_lariatCastObject.SetActive(false);
        }

        private IEnumerator ChainLightning()
        {
            vfx_lightningCastObject.SetActive(true);
            for (int i = 0; i < _chainLightningProjectilesPerAttack; i++)
            {
                FireProjectile(_chainLightning);
                _audio.PlaySound(lightningFireSfx);
                yield return _lightningInterval;
            }
            vfx_lightningCastObject.SetActive(false);
        }

        private IEnumerator CastingCoroutine()
        {
            // Wait some time for him to reach the top
            yield return new WaitForSeconds(3f);

            animator.Play("Casting");
            fancyCastTrail.SetActive(true);

            // Get a random sequence
            int sequenceIndex = Random.Range(0, castSequences.Count);

            SpellSequence sequence = castSequences[sequenceIndex];
            for (int tally = 0; tally < sequence.SpellID.Count; tally++)
            {

                if (sequence.SpellID[tally] < 100)
                {
                    // Cast FireBall Volley
                    spellController.CastFireball(sequence.SpellID[tally]);

                }
                else if (sequence.SpellID[tally] >= 100 && sequence.SpellID[tally] < 200)
                {
                    // Cast Fire Field
                    spellController.CastFireField(player.position.x, player.position.z);
                }

                yield return new WaitForSeconds(sequence.Delays[tally]);
            }

            ChangePhase();
        }

        private void OnCurrentHealthModified(int difference)
        {
            if (difference > 0)
            {
                return;
            }

            if (_health.CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            StopAllCoroutines();

            gameObject.SetActive(false);
            BossDied?.Invoke();
        }
    }
}