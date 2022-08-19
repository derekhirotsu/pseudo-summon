using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auto_Lariat : MonoBehaviour
{
    [SerializeField] protected float rotationSpeed = 200;
    float yRotation = 0;

    HealthTracker health;

    [SerializeField] List<TestBullet> childProjectiles;
    [SerializeField] private SoundFile hitSfx;
    private AudioSource audioSource;

    void Awake() {
        audioSource = this.GetComponent<AudioSource>();
    }

    void Start() {
        health = this.GetComponent<HealthTracker>();

        foreach (Transform child in transform) {
            TestBullet projectile = child.GetComponent<TestBullet>();

            if ( projectile != null) {
                childProjectiles.Add(projectile);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float diff = Time.fixedDeltaTime * this.rotationSpeed;
        yRotation = (yRotation + diff) % 360;
            
        // float yRotation = this.transform.rotation.y + (rotationSpeed * Time.fixedDeltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, yRotation, 0), 1f);

        if (health.TookDamage(consumeTrigger:true)) {
            PlaySoundEffect(hitSfx);
        }

        if (health.HealthPercentage <= 0) {
            CastOutward();
        }
    }

    protected void CastOutward() {
        this.transform.DetachChildren();

        for (int i = childProjectiles.Count-1; i >= 0; --i) {
            if (childProjectiles[i] != null) {
                Vector3 outwardVector = (childProjectiles[i].transform.position - this.transform.position).normalized;
                childProjectiles[i].SetDirection(outwardVector);
            }
        }

        Destroy(this.gameObject);
    }

    // Audio ------------------------------------------

    bool IsAudioValid(SoundFile soundFile) {
        if (audioSource == null || soundFile == null) {
            return false;
        }

        return true;
    }

    void SetAudioPitch(SoundFile soundFile) {
        audioSource.pitch = 1f;
        if (soundFile.randomizePitch) {
            audioSource.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
        }
    }

    void PlaySoundEffect(SoundFile soundFile) {
        if (!IsAudioValid(soundFile)) {
            return;
        }

        SetAudioPitch(soundFile);
        audioSource.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
    }
}
