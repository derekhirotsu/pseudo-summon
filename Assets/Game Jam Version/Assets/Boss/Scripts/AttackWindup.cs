using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWindup : MonoBehaviour
{
    [SerializeField] protected float windupTime = 1.2f;
    [SerializeField] protected AudioClip windupAudio;

    // Start is called before the first frame update
    void Start() {
        // play audio clip;
        Destroy(this.gameObject, windupTime);
    }
    
}
