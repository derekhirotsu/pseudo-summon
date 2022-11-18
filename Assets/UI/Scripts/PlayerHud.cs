using PseudoSummon.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class PlayerHud : MonoBehaviour
    {
        private HealthTracker _playerHealth;
        private PlayerController _playerController;

        [SerializeField] private PlayerHealthDisplay _playerHealthDisplay;
        [SerializeField] private PlayerBusterMeter _playerBusterMeter;

        private void OnEnable()
        {
            if (_playerHealth == null || _playerController == null)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            _playerHealthDisplay.SetHealthValue(_playerHealth.CurrentHealth);
            _playerBusterMeter.SetMeterFill(_playerController.BusterFillPercent);
        }

        public void SetPlayer(GameObject playerObject)
        {
            _playerHealth = playerObject.GetComponent<HealthTracker>();
            _playerController = playerObject.GetComponent<PlayerController>();

            if (_playerHealth == null || _playerController == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}
