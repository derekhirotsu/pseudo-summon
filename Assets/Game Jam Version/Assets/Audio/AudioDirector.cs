using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDirector : MonoBehaviour
{
    [SerializeField] AudioClip bossHitSound;
    [SerializeField] AudioClip bossBarrageSound;
    [SerializeField] AudioClip bossFireballSound;
    [SerializeField] AudioClip bossFireFieldChargeSound;
    [SerializeField] AudioClip bossFireFieldExplosionSound;
    [SerializeField] AudioClip bossLariatSound;
    [SerializeField] AudioClip bossLightningSound;
    [SerializeField] AudioClip bossWaveSound;
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip playerShootSound;
    [SerializeField] AudioClip playerDodgeSound;

    private AudioSource source;
    OptionsController options;

    public static AudioDirector instance;

    void Awake() {
        instance = this;
        source = GetComponent<AudioSource>();
        options = OptionsController.instance;
        source.volume = options.GetSfxVolume();
    }

    void OnEnable() {
        options.onSfxOptionsChange += SetVolumeLevel;
    }

    void OnDisable() {
        options.onSfxOptionsChange -= SetVolumeLevel;
    }

    public void SetVolumeLevel(float volume) {
        source.volume = volume;
    }

    public void PlayBossHitSound() {
        source.pitch = Random.Range(0.85f, 1.15f);
        source.PlayOneShot(bossHitSound, 0.75f * source.volume);
    }
    
    public void PlayBossShootBarrageSound() {
        source.pitch = Random.Range(0.8f, 1.35f);
        source.PlayOneShot(bossBarrageSound, 0.3f * source.volume);
    }

    public void PlayBossShootFireballSound() {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(bossFireballSound, 0.4f * source.volume);
    }

    public void PlayBossFireFieldChargeSound() {
        source.pitch = Random.Range(0.95f, 1.05f);
        source.PlayOneShot(bossFireFieldChargeSound, 0.5f * source.volume);
    }

    public void PlayBossFireFieldExplosionSound() {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(bossFireFieldExplosionSound, 0.5f * source.volume);
    }

    public void PlayBossShootLariatSound() {
        source.pitch = Random.Range(0.85f, 1.15f);
        source.PlayOneShot(bossLariatSound, 0.42f * source.volume);
    }

    public void PlayBossShootLightningSound() {
        source.pitch = Random.Range(0.99f, 1.01f);
        source.PlayOneShot(bossLightningSound, 0.27f * source.volume);
    }

    public void PlayBossShootWaveSound() {
        source.pitch = Random.Range(0.85f, 0.95f);
        source.PlayOneShot(bossWaveSound, 0.45f * source.volume);
    }

    public void PlayPlayerHitSound() {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(playerHitSound, 0.9f * source.volume);
    }

    public void PlayPlayerShootSound() {
        source.pitch = Random.Range(0.98f, 1.00f);
        source.PlayOneShot(playerShootSound, 0.20f * source.volume);
    }

    public void PlayPlayerDodgeSound() {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(playerDodgeSound, 0.6f * source.volume);
    }
}
