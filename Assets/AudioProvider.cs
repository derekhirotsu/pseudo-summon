using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProvider : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void SetPlayerAudioPitch(SoundFile soundFile)
    {
        _audioSource.pitch = 1f;
        if (soundFile.randomizePitch)
        {
            _audioSource.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
        }
    }

    public void PlaySound(SoundFile soundFile)
    {
        SetPlayerAudioPitch(soundFile);
        _audioSource.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
    }

    public void StopSound()
    {
        _audioSource.Stop();
    }
}
