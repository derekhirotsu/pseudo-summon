using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBusterAttack : MonoBehaviour
{
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float timeToLive;
    [SerializeField] LayerMask wallCollisionLayer;
    [SerializeField] protected LayerMask targetLayer;

    [SerializeField] protected GameObject optionalDeathParticle;
    protected bool directionSet = false;

    // The list of colliders currently inside the trigger
    [SerializeField] protected List<Collider> triggerList;

    Vector3 direction;

    public Action CallbackTest;

    public void SetCallback(Action callback)
    {
        CallbackTest = callback;
    }

    void Start()
    {
        triggerList = new List<Collider>();
        Destroy(this.gameObject, this.timeToLive);
    }

    void Update()
    {
        foreach (Collider col in triggerList)
        {
            if (col == null)
            {
                continue;
            }
            Debug.Log(this.name + " is colliding with " + col.name);
        }

        if (triggerList.Count <= 0)
        {
            Debug.LogWarning(this.name + " has no collisions ");
        }
    }

    void FixedUpdate()
    {
        this.transform.position += (transform.up * 0.1f * Time.unscaledDeltaTime);
    }

    void OnDisable()
    {
        if (optionalDeathParticle != null)
        {
            GameObject particle = Instantiate(optionalDeathParticle, this.transform.position, this.transform.rotation);
            Destroy(particle, 0.8f);
        }

        CallbackTest = null;
    }

    void OnTriggerEnter(Collider entity) {
        // Bullet hit target
        if (targetLayer.Contains(entity.gameObject.layer) && !triggerList.Contains(entity)) {

            triggerList.Add(entity);
        }
    }

    void OnTriggerExit(Collider entity) {
        // Bullet hit target
        if (triggerList.Contains(entity)) {

            triggerList.Remove(entity);
        }
    }

    void OnTriggerStay(Collider entity) {
        if (targetLayer.Contains(entity.gameObject.layer) && !triggerList.Contains(entity)) {

            triggerList.Add(entity);
        }
    }

    public void DamageAllCollisions() {

        CallbackTest?.Invoke();

        foreach (Collider target in triggerList) {
            if (target == null) {
                continue;
            }
            HealthTracker health = target.gameObject.GetComponent<HealthTracker>();
            if (health != null) {
                if (health.HealthPercentage < 0.2f) {
                    health.ModifyHealth(-bulletDamage * 2);
                } else {
                    health.ModifyHealth(-bulletDamage);
                }

                UI_ScoreTracker.Instance.AddScore(8000, multiply:true);
                
            } else {
                Debug.LogWarning(this.name + " collided with " + target.name + ", but no health component was found.");
            }
        }
    }

    public void SetDirection(Vector3 newDirection) {
        direction = newDirection;
        directionSet = true;
    }

    public void SetTargetLayer(LayerMask targets) {
        targetLayer = targets;
    }
}
