using System;
using PseudoSummon.UI;
using UnityEngine;

namespace PseudoSummon
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private PlayerHud _playerHud;
        [SerializeField] private ScoreTracker _scoreTracker;
        [SerializeField] private ScoreDisplay _scoreDisplay;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _boss;

        private HealthTracker _playerHealth;
        private HealthTracker _bossHealth;

        private void Awake()
        {
            _playerHealth = _player.GetComponent<HealthTracker>();
            _bossHealth = _boss.GetComponent<HealthTracker>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
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
        }

        private void Update()
        {
            _scoreTracker.DecayScore = _playerHealth.CurrentHealth > 0 && _bossHealth.CurrentHealth > 0;
            _scoreDisplay.SetScoreText(_scoreTracker.Score);
        }
    }
}
