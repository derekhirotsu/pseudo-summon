using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    // TODO: Refactor this; It can be abstracted along with TestBullet into a base projectile class
    public class FireballProjectile : MonoBehaviour
    {
        [SerializeField] int bulletDamage = 1;
        [SerializeField] LayerMask wallCollisionLayer;
        [SerializeField] protected LayerMask targetLayer;
        Vector3 direction;
        float projectileSpeed;

        void OnTriggerEnter(Collider entity)
        {
            if (wallCollisionLayer.Contains(entity.gameObject.layer))
            {
                Destroy(gameObject);
            }

            if (targetLayer.Contains(entity.gameObject.layer))
            {

                Health entityHealth = entity.gameObject.GetComponent<Health>();
                if (entityHealth != null)
                {
                    entityHealth.ModifyCurrentHealth(-bulletDamage);
                }

                Destroy(gameObject);
            }
        }

        void FixedUpdate()
        {
            transform.position += direction.normalized * projectileSpeed * Time.fixedDeltaTime;
        }

        public void SetDirection(Vector3 newDirection)
        {
            direction = newDirection;
        }

        public void SetSpeed(float speed)
        {
            projectileSpeed = speed;
        }
    }
}