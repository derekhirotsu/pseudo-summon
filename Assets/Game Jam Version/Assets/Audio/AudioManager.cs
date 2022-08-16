using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<SoundFile> soundFiles;

    private AudioSource audioSource;
    OptionsController options;

    public static AudioManager instance;

    void Awake() {
        options = OptionsController.instance;

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = options.GetSfxVolume();
    }

    void OnEnable() {
        options.onSfxOptionsChange += SetVolumeLevel;
    }

    void OnDisable() {
        options.onSfxOptionsChange -= SetVolumeLevel;
    }

    public void SetVolumeLevel(float volume) {
        audioSource.volume = volume;
    }

    public void PlayOneShotSoundFile(string audioName) {
        SoundFile sf = soundFiles.Find(sf => sf.audioName == audioName);
        if (sf.randomizePitch) {
            audioSource.pitch = Random.Range(sf.minPitch, sf.maxPitch);
        } else {
            audioSource.pitch = 1f;
        }

        audioSource.PlayOneShot(sf.audioClip, sf.volumeScale);
    }

    public void PlaySoundFile(string audioName) {
        SoundFile sf = soundFiles.Find(sf => sf.audioName == audioName);
        if (sf.randomizePitch) {
            audioSource.pitch = Random.Range(sf.minPitch, sf.maxPitch);
        } else {
            audioSource.pitch = 1f;
        }
        audioSource.clip = sf.audioClip;

        audioSource.Play();
    }

    public void StopSoundFile() {
        audioSource.Stop();
    }
}
