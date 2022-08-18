using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spell_FireField : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float spellDuration;
    [SerializeField] float spellArmTime;
    [SerializeField] GameObject inidicator;
    GameObject fireField;

    [SerializeField] private SoundFile chargeSfx;
    [SerializeField] private SoundFile detonateSfx;
    private AudioSource audioSource;

    void Awake() {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cast());
    }

    IEnumerator Cast() {
        this.fireField = Instantiate(this.projectilePrefab, this.transform.position, Quaternion.identity);
        this.fireField.transform.SetParent(this.gameObject.transform);
        this.fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(false);

        PlaySoundEffect(chargeSfx);

        yield return new WaitForSeconds(this.spellArmTime);
        
        Detonate();
        
        yield return new WaitForSeconds(this.spellDuration);

        Destroy(this.gameObject);
    }

    void Detonate() {
        Renderer r = this.fireField.GetComponent<Renderer>();
        Destroy(this.inidicator);
        PlaySoundEffect(detonateSfx);
        CapsuleCollider collider = this.fireField.GetComponent<CapsuleCollider>();
        this.fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(true);
        collider.enabled = true;
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
