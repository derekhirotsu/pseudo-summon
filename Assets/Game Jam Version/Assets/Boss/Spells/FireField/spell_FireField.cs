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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cast());
    }

    IEnumerator Cast() {
        this.fireField = Instantiate(this.projectilePrefab, this.transform.position, Quaternion.identity);
        this.fireField.transform.SetParent(this.gameObject.transform);
        this.fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(false);

        AudioManager.instance.PlayOneShotSoundFile("boss fire field charge");

        yield return new WaitForSeconds(this.spellArmTime);
        
        Detonate();
        
        yield return new WaitForSeconds(this.spellDuration);

        Destroy(this.gameObject);
    }

    void Detonate() {
        Renderer r = this.fireField.GetComponent<Renderer>();
        Destroy(this.inidicator);
        AudioManager.instance.PlayOneShotSoundFile("boss fire field fire");
        CapsuleCollider collider = this.fireField.GetComponent<CapsuleCollider>();
        this.fireField.GetComponent<FireFieldProjectile>().SetParticleEmission(true);
        collider.enabled = true;
    }
}
