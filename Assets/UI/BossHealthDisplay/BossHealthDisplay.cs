using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PseudoSummon.UI
{
    public class BossHealthDisplay : MonoBehaviour
    {
        [SerializeField] private Slider _bossHealthSlider;
        [SerializeField] private TMP_Text _bossNameDisplay;

        public void SetBossNameDisplayText(string name)
        {
            _bossNameDisplay.text = name;
        }

        public void SetHealthPercent(float value)
        {
            _bossHealthSlider.value = value;
        }
    }
}
