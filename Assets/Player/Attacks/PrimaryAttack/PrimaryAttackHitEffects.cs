using PseudoSummon.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class PrimaryAttackHitEffects : MonoBehaviour
    {
        [SerializeField] private SoundFile _hitSound;

        private ParticleSystem _particles;
        private AudioProvider _audioProvider;

        private void Awake()
        {
            _audioProvider = GetComponent<AudioProvider>();
            _particles = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            _audioProvider.PlaySound(_hitSound);
            Destroy(gameObject, _particles.main.duration + _particles.main.startLifetime.constantMax);
        }
    }
}
