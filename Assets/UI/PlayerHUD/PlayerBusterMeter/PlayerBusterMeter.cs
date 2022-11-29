using UnityEngine;
using UnityEngine.UI;

namespace PseudoSummon.UI
{
    public class PlayerBusterMeter : MonoBehaviour
    {
        [SerializeField] private Image _meterFill;
        [SerializeField] private Image _mouseIcon;
        [SerializeField] private Color _mouseIconUnchargedColor;
        [SerializeField] private Color _mouseIconChargedColor;

        public void SetMeterFill(float percent)
        {
            _meterFill.fillAmount = percent;
            _mouseIcon.color = _meterFill.fillAmount <= 0 ? _mouseIconChargedColor : _mouseIconUnchargedColor;
        }
    }
}
