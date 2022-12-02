using UnityEngine;

namespace PseudoSummon
{
    public class TestBullet : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed;
        [SerializeField] private int bulletDamage = 1;
        [SerializeField] private float timeToLive;
        [SerializeField] private LayerMask wallCollisionLayer;
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected GameObject optionalDeathParticle;

        private Vector3 direction;
        private float timeAlive = 0f;

        #region UnityFunctions

        private void FixedUpdate()
        {
            if (timeAlive > timeToLive)
            {
                HandleDestroy();
                return;
            }

            timeAlive += Time.fixedDeltaTime;

            transform.position += direction * bulletSpeed * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter(Collider entity)
        {
            if (wallCollisionLayer.Contains(entity.gameObject.layer))
            {
                HandleWallCollision(entity);
            }

            if (targetLayer.Contains(entity.gameObject.layer))
            {
                HandleTargetCollision(entity);
            }
        }

        #endregion

        public void SetDirection(Vector3 newDirection)
        {
            direction = newDirection.normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void SetTargetLayer(LayerMask targets)
        {
            targetLayer = targets;
        }

        private void HandleWallCollision(Collider entity)
        {
            HandleDestroy();
        }

        private void HandleTargetCollision(Collider entity)
        {
            Health health = entity.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.ModifyCurrentHealth(-bulletDamage);
            }

            HandleDestroy();
        }

        private void HandleDestroy()
        {
            if (optionalDeathParticle != null)
            {
                GameObject particle = Instantiate(optionalDeathParticle, transform.position, transform.rotation);
                Destroy(particle, 0.8f);
            }

            Destroy(gameObject);
        }
    }
}