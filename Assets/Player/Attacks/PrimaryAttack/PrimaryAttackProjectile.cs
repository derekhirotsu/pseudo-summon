using PseudoSummon.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class PrimaryAttackProjectile : Projectile
    {
        [SerializeField] private SoundFile _hitSound;
        [SerializeField] private GameObject _destructionParticle;

        private PlayerController _playerController;

        protected override void OnTriggerEnter(Collider entity)
        {
            if (!collisionMask.Contains(entity.gameObject.layer))
            {
                return;
            }

            if (_playerController && entity.gameObject.CompareTag("Boss"))
            {
                _playerController.OnBossHit();
            }

            Health health = entity.gameObject.GetComponent<Health>();

            if (health)
            {
                health.ModifyCurrentHealth(-damage);
            }

            Instantiate(_destructionParticle, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }
    }
}
