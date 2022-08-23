using UnityEngine;

namespace PseudoSummon
{
    [RequireComponent(typeof(AudioSource))]
    public class GenericAudioPlayer : MonoBehaviour, IAudioPlayer
    {
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundFile soundFile)
        {
            if (soundFile == null)
            {
                return;
            }

            audioSource.pitch = 1f;
            if (soundFile.randomizePitch)
            {
                audioSource.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
            }

            audioSource.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
        }

        public void StopSound()
        {
            audioSource?.Stop();
        }
    }
}
