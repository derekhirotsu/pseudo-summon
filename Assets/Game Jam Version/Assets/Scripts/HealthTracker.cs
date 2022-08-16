using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 200000;
    [SerializeField] protected int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }
    public float HealthPercentage { get { return ( float ) currentHealth / maxHealth; } }

    // Trigger to read for taking damage
    protected bool tookDamage = false;
    protected IEnumerator tookDamageTrigger;
    public bool TookDamage(bool consumeTrigger = false) {
        bool damageTaken = tookDamage;

        if (consumeTrigger) {
            tookDamage = false;
        }

        return damageTaken;
    }

    // Start is called before the first frame update
    void Awake() {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(int mod) {
        if (tookDamage) {
            return;
        }
        
        currentHealth += mod;

        if (mod < 0) {
            if (tookDamageTrigger != null) {
                StopCoroutine(tookDamageTrigger);
            }

            tookDamageTrigger = TookDamageTrigger();
            StartCoroutine(tookDamageTrigger);
        }
    }

    protected IEnumerator TookDamageTrigger() {
        tookDamage = true;

        yield return new WaitForSeconds(0.05f);

        tookDamage = false;
    }
}
