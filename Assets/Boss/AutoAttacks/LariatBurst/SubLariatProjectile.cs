using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class SubLariatProjectile : Projectile
    {
        public bool Launched = false;

        protected override void FixedUpdate()
        {
            if (Launched)
            {
                base.FixedUpdate();
            }
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
