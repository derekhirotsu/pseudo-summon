using System;
using PseudoSummon.Audio;
using PseudoSummon.UI;
using UnityEngine;

namespace PseudoSummon
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private PlayerHud _playerHud;
        [SerializeField] private BossHealthDisplay _bossHealthDisplay;
        [SerializeField] private ScoreTracker _scoreTracker;
        [SerializeField] private ScoreDisplay _scoreDisplay;
        [SerializeField] private PlayerCanvas UI;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _boss;
        [SerializeField] private string _bossName; // This could be set by some boss info if we had more bosses.

        private PlayerController _playerController;
        private BossController _bossController;

        private void Awake()
        {
            _playerController = _player.GetComponent<PlayerController>();
            _bossController = _boss.GetComponent<BossController>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            _playerController.PlayerDied += OnPlayerDied;
            _playerController.PausePressed += OnPausePressed;
            _playerController.ScoreModified += OnScoreModified;
            _bossController.BossDied += OnBossDied;
            UI_TutorialOnFirstPlay.Instance.TutorialEnded += OnTutorialEnded;
        }

        private void OnDisable()
        {
            _playerController.PlayerDied -= OnPlayerDied;
            _playerController.PausePressed -= OnPausePressed;
            _playerController.ScoreModified -= OnScoreModified;
            _bossController.BossDied -= OnBossDied;
            UI_TutorialOnFirstPlay.Instance.TutorialEnded -= OnTutorialEnded;
        }

        public void AddScore(int amount, bool increaseMultiplier = false)
        {
            _scoreTracker.AddScore(amount, increaseMultiplier);
        }

        public int GetScore()
        {
            return _scoreTracker.Score;
        }

        private void Start()
        {
            _playerHud.SetPlayer(_player);
            _bossHealthDisplay.SetBossNameDisplayText(_bossName); 

            if (UI_TutorialOnFirstPlay.Instance.FirstPlay)
            {
                UI_TutorialOnFirstPlay.Instance.StartTutorialSequence();
                _playerController.InTutorial = true;
            } 
        }

        private void Update()
        {
            _scoreDisplay.SetScoreText(_scoreTracker.Score);
            _bossHealthDisplay.SetHealthPercent(_bossController.Health.CurrentHealthPercent);
        }

        public void Pause()
        {
            Music.Instance.SetLowPassFilterEnabled(true);
            Time.timeScale = 0f;
            _playerController.IsPaused = true;
            UI.DisplayPauseUI();
        }

        public void Unpause()
        {
            Music.Instance.SetLowPassFilterEnabled(false);
            Time.timeScale = 1f;
            _playerController.IsPaused = false;
            UI.HidePauseUI();
        }

        private void OnTutorialEnded()
        {
            _playerController.InTutorial = false;
        }

        private void OnPlayerDied()
        {
            _scoreTracker.DecayScore = false;
            UI.DisplayDeathUI(false);
        }

        private void OnBossDied()
        {
            _scoreTracker.DecayScore = false;
            _playerController.CanDie = false;
            UI.DisplayDeathUI(true);
            _playerController.enabled = false;
            AddScore(Mathf.RoundToInt(_scoreTracker.Score * _playerController.Health.CurrentHealthPercent));
        }

        private void OnPausePressed()
        {
            if (_playerController.IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        private void OnScoreModified(int value, bool multiply)
        {
            _scoreTracker.AddScore(value, multiply);
        }
    }
}
