using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auto_Lariat : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200;
    [SerializeField] private SoundFile hitSfx;
    [SerializeField] private List<TestBullet> childProjectiles;

    private HealthTracker health;
    private AudioProvider _audio;

    private float yRotation = 0;

    private void Awake() {
        _audio = GetComponent<AudioProvider>();
        health = GetComponent<HealthTracker>();
    }

    private void Start() {
        foreach (Transform child in transform) {
            TestBullet projectile = child.GetComponent<TestBullet>();

            if ( projectile != null) {
                childProjectiles.Add(projectile);
            }
        }
    }

    private void FixedUpdate()
    {
        float diff = Time.fixedDeltaTime * rotationSpeed;
        yRotation = (yRotation + diff) % 360;
            
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yRotation, 0), 1f);

        if (health.TookDamage(consumeTrigger:true)) {
            _audio.PlaySound(hitSfx);
        }

        if (health.HealthPercentage <= 0) {
            CastOutward();
        }
    }

    private void CastOutward() {
        transform.DetachChildren();

        for (int i = childProjectiles.Count-1; i >= 0; --i) {
            if (childProjectiles[i] != null) {
                Vector3 outwardVector = (childProjectiles[i].transform.position - transform.position).normalized;
                childProjectiles[i].SetDirection(outwardVector);
            }
        }

        Destroy(gameObject);
    }
}
