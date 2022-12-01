using UnityEngine;
using TMPro;

namespace PseudoSummon.UI
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _subText;
        [SerializeField] private TMP_Text _retryButtonText;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private TMP_Text _scoreTipText;

        public void DisplayMenuWin(int finalScore)
        {
            _mainText.text = "Vanquished!";
            _subText.text = "Your Resurrection was Achieved";
            _retryButtonText.text = "Once More!";
            _scoreTipText.gameObject.SetActive(true);
            _finalScoreText.text = "Final Score : - " + finalScore + " - ";
        }

        public void DisplayMenuLose(int finalScore)
        {
            _mainText.text = "Banished...";
            _subText.text = "Your Resurrection was Denied";
            _retryButtonText.text = "Retry";
            _finalScoreText.text = "Final Score : - " + finalScore + " - ";
        }
    }
}
