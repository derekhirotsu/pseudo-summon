using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float timeToLive;
    [SerializeField] LayerMask wallCollisionLayer;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected GameObject optionalDeathParticle;
    protected bool directionSet = false;

    Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, this.timeToLive);
    }

    void OnTriggerEnter(Collider entity) {

        // Bullet hit wall
        if (wallCollisionLayer.Contains(entity.gameObject.layer)) {
            Destroy(this.gameObject);
        }

        // Bullet hit target
        if (targetLayer.Contains(entity.gameObject.layer)) {
            HealthTracker health = entity.gameObject.GetComponent<HealthTracker>();
            
            if (health != null) {
                health.ModifyHealth(-bulletDamage);
            } else {
                Debug.LogWarning(this.name + " collided with " + entity.name + ", but no health component was found.");
            }

            Destroy(this.gameObject);
        }

    }

    void FixedUpdate() {
        this.transform.position += (direction.normalized * this.bulletSpeed * Time.fixedDeltaTime);

        if (directionSet) {
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    public void SetDirection(Vector3 newDirection) {
        direction = newDirection;
        directionSet = true;
    }

    public void SetTargetLayer(LayerMask targets) {
        targetLayer = targets;
    }

    void OnDisable() {
        if (optionalDeathParticle != null) {
            GameObject particle = Instantiate (optionalDeathParticle, this.transform.position, this.transform.rotation);
            Destroy(particle, 0.8f);
        }
    }
}
