using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using PseudoSummon;
using PseudoSummon.Audio;
using TMPro;

namespace PseudoSummon.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown musicTrackDropdown;
        [SerializeField] private AudioMixer mixer;
        private const string MIXER_MUSIC_VOLUME = "musicVolume";
        private const string MIXER_SFX_VOLUME = "sfxVolume";

        private void Start()
        {
            musicTrackDropdown.value = Music.Instance.CurrentTrackIndex;

            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        private void OnEnable()
        {
            musicVolumeSlider.value = GetMusicVolume();
            sfxVolumeSlider.value = GetSfxVolume();
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        public void OnGraphicsQualitySelection(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void OnMusicTrackSelect(int trackNumber)
        {
            Music.Instance.SetMusicTrack(trackNumber);
        }

        private void SetMusicVolume(float value)
        {
            mixer.SetFloat(MIXER_MUSIC_VOLUME, VolumeToDb(value));
        }

        private void SetSfxVolume(float value)
        {
            mixer.SetFloat(MIXER_SFX_VOLUME, VolumeToDb(value));
        }

        private float GetMusicVolume()
        {
            float musicVolumeDb = 0.0f;
            mixer.GetFloat(MIXER_MUSIC_VOLUME, out musicVolumeDb);
            return DbToVolume(musicVolumeDb);
        }

        private float GetSfxVolume()
        {
            float sfxVolumeDb = 0.0f;
            mixer.GetFloat(MIXER_SFX_VOLUME, out sfxVolumeDb);
            return DbToVolume(sfxVolumeDb);
        }

        private float DbToVolume(float dB)
        {
            return Mathf.Pow(10.0f, 0.05f * dB);
        }

        private float VolumeToDb(float volume)
        {
            return 20.0f * Mathf.Log10(volume);
        }
    }
}