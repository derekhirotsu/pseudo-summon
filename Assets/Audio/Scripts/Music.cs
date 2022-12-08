using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PseudoSummon.Audio
{
    public class Music : MonoBehaviour
    {
        [SerializeField] private AudioClip _titleMusic;
        [SerializeField] private List<AudioClip> _musicTracks;

        private AudioSource source;
        private AudioLowPassFilter lowPassFilter;

        public int CurrentTrackIndex { get; private set; }
        public static Music Instance { get; private set; }

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            lowPassFilter = GetComponent<AudioLowPassFilter>();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        protected void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("StartScene"))
            {
                PlayTitleMusic();
            }
            else
            {
                SetMusicTrack(CurrentTrackIndex);
            }
        }

        public void SetMusicTrack(int trackIndex)
        {
            int index = Mathf.Clamp(trackIndex, 0, _musicTracks.Count - 1);
            AudioClip selectedTrack = _musicTracks[index];

            if (selectedTrack == source.clip)
            {
                return;
            }

            CurrentTrackIndex = index;

            // Don't change music in the main menu scene.
            if (SceneManager.GetActiveScene().name != "StartScene")
            {
                source.clip = selectedTrack;
                source.Play();
            }
        }

        public void SetLowPassFilterEnabled(bool enabled)
        {
            lowPassFilter.enabled = enabled;
        }

        private void PlayTitleMusic()
        {
            source.clip = _titleMusic;
            source.PlayDelayed(2f);
        }
    }
}
