using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class ScoreTracker : MonoBehaviour
    {
        [SerializeField] private float _multiplierTime = 3f;
        [SerializeField] private float _multiplierModifier = 0.06f;
        [SerializeField] private float _scoreDecayTime = 5f;
        [SerializeField] private int _scoreDecayAmount = -100;

        private int _score = 0;
        public int Score { get { return _score; } }

        private float _currentScoreMultiplier = 1f;
        private float _timeToScoreDecay = 0f;
        private WaitForSeconds _multiplierModifierWait;

        public bool DecayScore = true;

        private void Start()
        {
            _multiplierModifierWait = new WaitForSeconds(_multiplierTime);
        }

        private void Update()
        {
            if (DecayScore)
            {
                _timeToScoreDecay -= Time.deltaTime;

                if (_timeToScoreDecay <= 0f)
                {
                    AddScore(_scoreDecayAmount);
                    _timeToScoreDecay = _scoreDecayTime;
                }
            }
        }

        public void AddScore(int amount, bool increaseMultiplier = false)
        {
            _score = (int) Mathf.Clamp(_score + (amount * _currentScoreMultiplier), 0, Mathf.Infinity);

            if (increaseMultiplier)
            {
                StartCoroutine(ModifyScoreMultiplierCoroutine());
            }
        }

        private IEnumerator ModifyScoreMultiplierCoroutine()
        {
            _currentScoreMultiplier += _multiplierModifier;

            yield return _multiplierModifierWait;

            _currentScoreMultiplier -= _multiplierModifier;
        }
    }
}
