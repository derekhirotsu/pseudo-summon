using UnityEngine;
using UnityEngine.UI;

namespace PseudoSummon.UI
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text _healthNumberText;

        public void SetHealthValue(int value)
        {
            _healthNumberText.text = value.ToString();
        }
    }
}
