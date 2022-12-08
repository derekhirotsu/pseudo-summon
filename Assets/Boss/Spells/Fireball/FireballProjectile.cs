using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class FireballProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider entity)
        {
            if (!collisionMask.Contains(entity.gameObject.layer))
            {
                return;
            }

            Health entityHealth = entity.gameObject.GetComponent<Health>();

            if (entityHealth)
            {
                entityHealth.ModifyCurrentHealth(-damage);
            }

            Destroy(gameObject);
        }
    }
}