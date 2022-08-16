using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject firefield;

    public void CastFireball(int pattern) {
        GameObject go = Instantiate(this.fireball);
        spell_Fireball fireball = go.GetComponent<spell_Fireball>();
        fireball.Cast(pattern);
    }

    public void CastFireField(float x, float z) {
        Vector3 location =  new Vector3(x, this.firefield.transform.position.y, z);

        Instantiate(this.firefield, location, Quaternion.identity);
    }
}
