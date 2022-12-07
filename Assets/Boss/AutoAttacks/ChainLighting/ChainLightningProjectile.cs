using UnityEngine;

namespace PseudoSummon
{
    public class ChainLightningProjectile : Projectile
    {
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
