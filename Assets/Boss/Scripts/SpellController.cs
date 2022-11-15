using UnityEngine;

public class SpellController : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject firefield;

    public void CastFireball(int pattern) {
        GameObject go = Instantiate(fireball);
        spell_Fireball newFireball = go.GetComponent<spell_Fireball>();
        newFireball.Cast(pattern);
    }

    public void CastFireField(float x, float z) {
        Vector3 location =  new Vector3(x, firefield.transform.position.y, z);

        Instantiate(firefield, location, Quaternion.identity);
    }
}
