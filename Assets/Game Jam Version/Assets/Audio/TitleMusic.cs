using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusic : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(WaitToPlay());
    }

    IEnumerator WaitToPlay() {
        yield return new WaitForSeconds(2f);
        source.Play();
    }
}
