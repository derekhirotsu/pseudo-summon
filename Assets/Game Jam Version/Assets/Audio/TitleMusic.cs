using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusic : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.volume = OptionsController.instance.GetMusicVolume();
        StartCoroutine(WaitToPlay());
    }

    void OnEnable() {
        OptionsController.instance.onMusicOptionsChange += SetVolumeLevel;
    }

    void OnDisable() {
        OptionsController.instance.onMusicOptionsChange -= SetVolumeLevel;
    }

    IEnumerator WaitToPlay() {
        yield return new WaitForSeconds(2f);
        source.Play();
    }

    public void SetVolumeLevel(float volume) {
        source.volume = volume;
    }
}
