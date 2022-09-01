using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusic : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(WaitToPlay());
    }

    private IEnumerator WaitToPlay() {
        yield return new WaitForSeconds(2f);
        source.Play();
    }
}
