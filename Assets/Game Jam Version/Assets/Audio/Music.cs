using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    [SerializeField] List<AudioClip> musicTracks;

    private AudioSource source;
    private AudioLowPassFilter lowPassFilter;

    public static Music instance;

    OptionsController options;

    void Awake()
    {
        options = OptionsController.instance;
        source = GetComponent<AudioSource>();
        SetMusicTrack(options.GetMusicTrack());
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        source.volume = options.GetMusicVolume();
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
        } else {
            Destroy(this.gameObject);
        }    
    }
    
    protected void PlaySong() {
        if (source.isPlaying) {
            source.loop = true;
            return;
        }

        source.loop = true;
        source.Play();
    }

    void OnEnable() {
        
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnSceneLoad;
        options.onMusicOptionsChange += SetVolumeLevel;
        options.onMusicTrackChange += SetMusicTrack;
    }

    void OnDisable() {
        
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded -= OnSceneLoad;
        options.onMusicOptionsChange -= SetVolumeLevel;
        options.onMusicTrackChange -= SetMusicTrack;
    }

    protected void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        // Scene mScene = SceneManager.GetActiveScene();
        if (scene.name.Equals("StartScene")) {
            Destroy(this.gameObject);
        } else { 
            PlaySong();
        }
    }

    public void SetMusicTrack(int trackIndex) {
        int index = Mathf.Clamp(trackIndex, 0, musicTracks.Count - 1);
        AudioClip selectedTrack = musicTracks[index];

        if (selectedTrack == source.clip) {
            return;
        }

        source.clip = selectedTrack;
        source.Play();
    }

    public void SetVolumeLevel(float volume) {
        source.volume = volume;
    }

    public void SetLowPassFilterEnabled(bool enabled) {
        lowPassFilter.enabled = enabled;
    } 
}
