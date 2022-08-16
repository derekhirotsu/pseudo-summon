using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [SerializeField] config_Options Options;
    public static OptionsController instance;


    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(instance);
    }

    public delegate void OnMusicOptionsChange(float value);
    public OnMusicOptionsChange onMusicOptionsChange;

    public void OnMusicVolumeChange(float value) {
        Options.musicVolume = Mathf.Clamp(value, 0f, 1f);

        if (onMusicOptionsChange != null) {
            onMusicOptionsChange(value);
        }
    }

    public delegate void OnMusicTrackChange(int trackNumber);
    public OnMusicTrackChange onMusicTrackChange;

    public void OnMusicChangeTrack(int trackNumber) {
        Options.musicTrack = trackNumber;
        if (onMusicTrackChange != null) {
            onMusicTrackChange(trackNumber);
        }
    }

    public delegate void OnSfxOptionsChange(float value);
    public OnSfxOptionsChange onSfxOptionsChange;

    public void OnSfxVolumeChange(float value) {
        Options.sfxVolume = Mathf.Clamp(value, 0f, 1f);

        if (onSfxOptionsChange != null) {
            onSfxOptionsChange(value);
        }
    }

    public float GetMusicVolume() {
        return Options.musicVolume;
    }

    public float GetSfxVolume() {
        return Options.sfxVolume;
    }

    public int GetMusicTrack() {
        return Options.musicTrack;
    }
}
