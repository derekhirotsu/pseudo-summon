using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown musicTrackDropdown;
    [SerializeField] private AudioMixer mixer;

    const string MIXER_MUSIC_VOLUME = "musicVolume";
    const string MIXER_SFX_VOLUME = "sfxVolume";

    void Awake() {
        musicTrackDropdown.value = OptionsController.instance.GetMusicTrack();

        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    void OnEnable() {
        musicVolumeSlider.value = GetMusicVolume();
        sfxVolumeSlider.value = GetSfxVolume();
        qualityDropdown.value = QualitySettings.GetQualityLevel(); 
    }
    
    public void OnGraphicsQualitySelection(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void OnMusicTrackSelect(int trackNumber) {
        OptionsController.instance.OnMusicChangeTrack(trackNumber);
    }

    void SetMusicVolume(float value) {
        mixer.SetFloat(MIXER_MUSIC_VOLUME, VolumeToDb(value));
    }

    void SetSfxVolume(float value) {
        mixer.SetFloat(MIXER_SFX_VOLUME, VolumeToDb(value));
    }

    float GetMusicVolume() {
        float musicVolumeDb = 0.0f;
        mixer.GetFloat(MIXER_MUSIC_VOLUME, out musicVolumeDb);
        return DbToVolume(musicVolumeDb);
    }

    float GetSfxVolume() {
        float sfxVolumeDb = 0.0f;
        mixer.GetFloat(MIXER_SFX_VOLUME, out sfxVolumeDb);
        return DbToVolume(sfxVolumeDb);        
    }
 
    float DbToVolume(float dB) {
        return Mathf.Pow(10.0f, 0.05f * dB);
    }

    float VolumeToDb(float volume) {
        return 20.0f * Mathf.Log10(volume);
    }
}
