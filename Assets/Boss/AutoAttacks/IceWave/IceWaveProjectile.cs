using UnityEngine;

namespace PseudoSummon
{
    public class IceWaveProjectile : Projectile
    {
        [SerializeField] private float _rotationSpeed;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            transform.Rotate(0, 0, _rotationSpeed * Time.fixedDeltaTime);
        }

        protected override void OnTriggerEnter(Collider entity)
        {
            if (collisionMask.Contains(entity.gameObject.layer))
            {
                Health health = entity.gameObject.GetComponent<Health>();

                if (health)
                {
                    health.ModifyCurrentHealth(-damage);
                }

                Destroy(gameObject);
            }
        }
    }
}
