using UnityEngine;

public class AttackWindup : MonoBehaviour
{
    [SerializeField] protected float windupTime = 1.2f;
    [SerializeField] protected AudioClip windupAudio;

    private void Start() {
        Destroy(gameObject, windupTime);
    }
    
}
