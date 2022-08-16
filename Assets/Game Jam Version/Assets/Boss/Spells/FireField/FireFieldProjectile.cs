using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFieldProjectile : MonoBehaviour
{
    [SerializeField] int bulletDamage = 1;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] ParticleSystem particles;

    void Start() {
        this.transform.localScale = new Vector3(0f, 0.05f, 0f);
    }

    void FixedUpdate() {
        if (this.transform.localScale.x < 1f) {
            this.transform.localScale += new Vector3(0.02f, 0f, 0.02f);
        }
    }

    void OnTriggerStay(Collider entity) {
        // Bullet hit target
        if (targetLayer.Contains(entity.gameObject.layer)) {
            HealthTracker health = entity.gameObject.GetComponent<HealthTracker>();
            
            if (health != null) {
                health.ModifyHealth(-bulletDamage);
            } else {
                Debug.LogWarning(this.name + " collided with " + entity.name + ", but no health component was found.");
            }

            // Destroy(this.gameObject);
        }
    }

    public void SetParticleEmission(bool shouldEmit) {
        var em = this.particles.emission;
        em.enabled = shouldEmit;
    }  
}
