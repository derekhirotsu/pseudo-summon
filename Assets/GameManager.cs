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
        [SerializeField] protected PlayerCanvas UI;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _boss;

        private PlayerController _playerController;
        private BossController _bossController;
        private HealthTracker _playerHealth;
        private HealthTracker _bossHealth;

        private void Awake()
        {
            _playerController = _player.GetComponent<PlayerController>();
            _playerHealth = _player.GetComponent<HealthTracker>();
            _bossController = _boss.GetComponent<BossController>();
            _bossHealth = _boss.GetComponentInChildren<HealthTracker>();

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
            _bossController.BossDied += OnBossDied;
        }

        private void OnDisable()
        {
            _playerController.PlayerDied -= OnPlayerDied;
            _bossController.BossDied -= OnBossDied;
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
            _bossHealthDisplay.SetBossNameDisplayText("Malakor the Miffed"); // This could be set by some boss info if we had more bosses.
        }

        private void Update()
        {
            _scoreTracker.DecayScore = _playerHealth.CurrentHealth > 0 && _bossHealth.CurrentHealth > 0;
            _scoreDisplay.SetScoreText(_scoreTracker.Score);
            _bossHealthDisplay.SetHealthPercent(_bossHealth.HealthPercentage);
        }

        private void OnPlayerDied()
        {
            UI.DisplayDeathUI(false);
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
            UI.HideOptionsUI();
        }

        private void OnBossDied()
        {
            _playerController.CanDie = false;
            UI.DisplayDeathUI(true);
            _playerController.enabled = false;

            AddScore(Mathf.RoundToInt(_scoreTracker.Score * _playerHealth.HealthPercentage));
        }
    }
}
