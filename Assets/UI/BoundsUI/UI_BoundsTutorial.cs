using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PseudoSummon.UI
{
    public class UI_BoundsTutorial : MonoBehaviour
    {
        [SerializeField] private List<string> messages;
        [SerializeField] private TMP_Text textLabel;

        public void DisplayTutorial(float textDelay)
        {
            StartCoroutine(ToggleThroughText(textDelay));
        }

        private IEnumerator ToggleThroughText(float delay)
        {

            textLabel.text = messages[0] + "\n\n \n\n ";
            yield return new WaitForSecondsRealtime(delay);

            textLabel.text = messages[0] + "\n\n" + messages[1] + "\n\n ";
            yield return new WaitForSecondsRealtime(delay);

            textLabel.text = messages[0] + "\n\n" + messages[1] + "\n\n" + messages[2];
            yield return new WaitForSecondsRealtime(delay);

        }
    }
}