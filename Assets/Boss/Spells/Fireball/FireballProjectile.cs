using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Bullet hit wall
        if (wallCollisionLayer.Contains(entity.gameObject.layer))
        {
            Destroy(this.gameObject);
        }

        // Bullet hit target
        if (targetLayer.Contains(entity.gameObject.layer))
        {
            HealthTracker health = entity.gameObject.GetComponent<HealthTracker>();

            if (health != null)
            {
                health.ModifyHealth(-bulletDamage);
            }
            else
            {
                Debug.LogWarning(this.name + " collided with " + entity.name + ", but no health component was found.");
            }

            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        this.transform.position += (direction.normalized * this.projectileSpeed * Time.fixedDeltaTime);
    }

    public void SetDirection(Vector3 newDirection)
    {
        this.direction = newDirection;
    }

    public void SetSpeed(float speed)
    {
        this.projectileSpeed = speed;
    }
}
