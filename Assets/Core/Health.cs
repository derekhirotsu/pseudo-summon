using UnityEngine;

namespace PseudoSummon
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _currentHealth;

        public int CurrentHealth { get { return _currentHealth; } }
        public float CurrentHealthPercent { get { return (float)_currentHealth / _maxHealth; } }

        public System.Action<int> CurrentHealthModified;

        public void ModifyCurrentHealth(int modifierValue)
        {
            _currentHealth += modifierValue;
            CurrentHealthModified?.Invoke(modifierValue);
        }
    }
}
