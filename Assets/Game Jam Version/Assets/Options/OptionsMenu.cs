using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown musicTrackDropdown;

    void Awake() {
        musicTrackDropdown.value = OptionsController.instance.GetMusicTrack();
    }

    // Start is called before the first frame update
    void Start()
    {
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeSliderChanged(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeSliderChanged(); });
    }

    void OnEnable() {
        musicVolumeSlider.value = OptionsController.instance.GetMusicVolume();
        sfxVolumeSlider.value = OptionsController.instance.GetSfxVolume();
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        
    }
    
    public void OnGraphicsQualitySelection(int qualityIndex) {
        
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void OnMusicVolumeSliderChanged() {
        // Debug.Log("music volume:" + musicVolumeSlider.value);
        OptionsController.instance.OnMusicVolumeChange(musicVolumeSlider.value);
    }

    public void OnSFXVolumeSliderChanged() {
        // Debug.Log("sfx volume:" + sfxVolumeSlider.value);
        OptionsController.instance.OnSfxVolumeChange(sfxVolumeSlider.value);
    }

    public void OnMusicTrackSelect(int trackNumber) {
        OptionsController.instance.OnMusicChangeTrack(trackNumber);
    } 
}
