using UnityEngine;
using TMPro;

namespace PseudoSummon.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        public void SetScoreText(int value)
        {
            Debug.Log(value.ToString());
            _scoreText.text = value.ToString();
        }
    }
}
