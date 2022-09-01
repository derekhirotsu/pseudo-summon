using System.Collections;
using UnityEngine;

public class spell_FireField : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float spellDuration;
    [SerializeField] private float spellArmTime;
    [SerializeField] private GameObject inidicator;
    private GameObject fireField;

    [SerializeField] private SoundFile chargeSfx;
    [SerializeField] private SoundFile detonateSfx;
    private AudioSource audioSource;

    private WaitForSeconds spellArmTimeWait;
    private WaitForSeconds spellDurationWait;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        spellArmTimeWait = new WaitForSeconds(spellArmTime);
        spellDurationWait = new WaitForSeconds(spellDuration);
        StartCoroutine(Cast());
    }

    private IEnumerator Cast() {
        fireField = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        fireField.transform.SetParent(gameObject.transform);
        fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(false);

        PlaySoundEffect(chargeSfx);

        yield return spellArmTimeWait;

        Detonate();

        yield return spellDurationWait;

        Destroy(gameObject);
    }

    private void Detonate() {
        Renderer r = fireField.GetComponent<Renderer>();
        Destroy(inidicator);
        PlaySoundEffect(detonateSfx);
        CapsuleCollider collider = fireField.GetComponent<CapsuleCollider>();
        fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(true);
        collider.enabled = true;
    }

    // Audio ------------------------------------------

    private bool IsAudioValid(SoundFile soundFile) {
        if (audioSource == null || soundFile == null) {
            return false;
        }

        return true;
    }

    private void SetAudioPitch(SoundFile soundFile) {
        audioSource.pitch = 1f;
        if (soundFile.randomizePitch) {
            audioSource.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
        }
    }

    private void PlaySoundEffect(SoundFile soundFile) {
        if (!IsAudioValid(soundFile)) {
            return;
        }

        SetAudioPitch(soundFile);
        audioSource.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
    }
}
