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

        [Header("Missile Attacks")]
        [SerializeField] private GameObject magicMissile;
        [SerializeField] private Projectile _magicMissile;
        [SerializeField] private GameObject vfx_missileWindup;
        [SerializeField] private GameObject vfx_missileCastObject;

        [Header("Lightning Attacks")]
        [SerializeField] private GameObject chainLightning;
        [SerializeField] private Projectile _chainLightning;
        [SerializeField] private GameObject vfx_lightningWindup;
        [SerializeField] private GameObject vfx_lightningCastObject;

        [Header("Purple Attacks")]
        [SerializeField] private GameObject lariatBurst;
        [SerializeField] private Projectile _lariat;
        [SerializeField] private GameObject vfx_lariatWindup;
        [SerializeField] private GameObject vfx_lariatCast;
        [SerializeField] private GameObject vfx_lariatCastObject;

        [Header("Ice Attacks")]
        [SerializeField] private GameObject iceShard;
        [SerializeField] private Projectile _iceWave;
        [SerializeField] private GameObject vfx_iceShardWindup;
        [SerializeField] private GameObject vfx_iceShardCastObject;
        [SerializeField] private GameObject vfx_iceShardCast;

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

        private float rotationSpeed;
        protected bool autoPhase = false;
        private bool clockwise = true;
        private float yRotation = 0;
        private float currentPhaseTime = 10;

        private SpellController spellController;

        [Header("Boss Audio")]
        [SerializeField] private SoundFile autoAttackChargeSfx;
        [SerializeField] private SoundFile iceWaveFireSfx;
        [SerializeField] private SoundFile missileBarrageFireSfx;
        [SerializeField] private SoundFile lariatFireSfx;
        [SerializeField] private SoundFile lightningFireSfx;
        private AudioProvider _audio;

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
                float diff = Time.deltaTime * rotationSpeed;
                if (clockwise)
                {
                    yRotation = (yRotation + diff) % 360;
                }
                else
                {
                    yRotation = (yRotation - diff) % 360;
                }
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
            currentPhaseTime = 10f + Random.Range(2f, 5f);
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

            currentPhaseTime = 15f + Random.Range(2f, 5f);
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
            while (true)
            {
                float test = Random.Range(minRotationSpeed, maxRotationSpeed);
                float test0 = Mathf.Floor(test);

                rotationSpeed = test0;

                yield return new WaitForSeconds(4f);
            }
        }

        private IEnumerator RandomizeRotationDirection()
        {
            while (true)
            {
                clockwise = Random.Range(0f, 1f) < 0.5f;

                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
        }

        private IEnumerator FiringCoroutine()
        {
            while (canFire)
            {

                animator.Play("AutoWindup");

                int result = Random.Range(0, 4);

                switch (0)
                {
                    case 0:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_missileWindup, autoWindupOrb.transform);
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(MissileBarrage());
                        break;

                    case 1:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_lightningWindup, autoWindupOrb.transform);
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(ChainLightning());
                        break;

                    case 2:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_lariatWindup, autoWindupOrb.transform);
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(LariatBurst());
                        break;

                    case 3:
                        _audio.PlaySound(autoAttackChargeSfx);
                        Instantiate(vfx_iceShardWindup, autoWindupOrb.transform);
                        yield return new WaitForSeconds(0.5f);
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

        //private void Fire(GameObject projectile, Vector3 positionOffset, Vector3 rotationOffset, float deviation = 0)
        //{
        //    Vector3 aimVector = (player.position - boss.position).normalized;
        //    aimVector.y = 0;
        //    Vector3 spawnLocation = boss.position + positionOffset + aimVector * 1;

        //    TestBullet newBullet = Instantiate(projectile, spawnLocation, Quaternion.identity).GetComponent<TestBullet>();
        //    newBullet.SetTargetLayer(bossTargets);

        //    // Random deviation
        //    aimVector.x += Random.Range(-deviation, deviation);
        //    aimVector.z += Random.Range(-deviation, deviation);
        //    aimVector = aimVector.normalized;

        //    newBullet.SetDirection(aimVector);
        //}

        private void FireProjectile(Projectile projectile, Vector3 originOffset, float degreeDeviation = 0)
        {
            Vector3 aimVector = (player.position - boss.position).normalized;
            aimVector.y = 0;
            Vector3 spawnLocation = boss.position + originOffset + aimVector * 1;

            aimVector = Quaternion.Euler(0, Random.Range(-degreeDeviation, degreeDeviation), 0) * aimVector;

            Projectile newProjectile = Instantiate(projectile, spawnLocation, Quaternion.identity).GetComponent<Projectile>();
            newProjectile.SetDirection(aimVector);
        } 

        private IEnumerator IceWaveVolley()
        {
            WaitForSeconds volleyInterval = new WaitForSeconds(1f);

            vfx_iceShardCastObject.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                _audio.PlaySound(iceWaveFireSfx);
                Instantiate(vfx_iceShardCast, autoWindupOrb.transform);
                for (int j = 0; j < 40; j += 1)
                {
                    //Fire(iceShard, Vector3.zero, Vector3.zero, 0.8f);
                    FireProjectile(_iceWave, Vector3.zero, 60f);
                }
                yield return volleyInterval;
            }
            vfx_iceShardCastObject.SetActive(false);
        }

        private IEnumerator MissileBarrage()
        {
            WaitForSeconds barrageInterval = new WaitForSeconds(0.08f);

            vfx_missileCastObject.SetActive(true);
            for (int i = 0; i < 60; i++)
            {
                //Fire(magicMissile, Vector3.zero, Vector3.zero, 0.8f);
                FireProjectile(_magicMissile, Vector3.zero, 60f);
                _audio.PlaySound(missileBarrageFireSfx);
                yield return barrageInterval;
            }
            vfx_missileCastObject.SetActive(false);
        }

        private IEnumerator LariatBurst()
        {
            WaitForSeconds lariatInterval = new WaitForSeconds(1f);

            vfx_lariatCastObject.SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                Instantiate(vfx_lariatCast, autoWindupOrb.transform);
                //Fire(lariatBurst, Vector3.zero, Vector3.zero);
                FireProjectile(_lariat, Vector3.zero);
                _audio.PlaySound(lariatFireSfx);
                yield return lariatInterval;
            }
            vfx_lariatCastObject.SetActive(false);
        }

        private IEnumerator ChainLightning()
        {
            WaitForSeconds lightningInterval = new WaitForSeconds(0.05f);
            Vector3 offset = new Vector3(3, 1, 0);

            vfx_lightningCastObject.SetActive(true);
            for (int i = 0; i < 200; i++)
            {
                FireProjectile(_chainLightning, offset);
                //Fire(chainLightning, offset, Vector3.zero);
                _audio.PlaySound(lightningFireSfx);
                yield return lightningInterval;
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