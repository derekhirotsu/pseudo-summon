using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon.UI
{
    public class PlayerHud : MonoBehaviour
    {
        private PlayerController _playerController;
        private Health _playerHealth;

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
            _playerController = playerObject.GetComponent<PlayerController>();
            _playerHealth = _playerController.Health;

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
