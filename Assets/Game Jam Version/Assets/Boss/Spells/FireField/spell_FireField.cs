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
    private AudioProvider _audio;

    private WaitForSeconds spellArmTimeWait;
    private WaitForSeconds spellDurationWait;

    private void Awake() {
        _audio = GetComponent<AudioProvider>();
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

        _audio.PlaySound(chargeSfx);

        yield return spellArmTimeWait;

        Detonate();

        yield return spellDurationWait;

        Destroy(gameObject);
    }

    private void Detonate() {
        Renderer r = fireField.GetComponent<Renderer>();
        Destroy(inidicator);
        _audio.PlaySound(detonateSfx);
        CapsuleCollider collider = fireField.GetComponent<CapsuleCollider>();
        fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(true);
        collider.enabled = true;
    }
}
