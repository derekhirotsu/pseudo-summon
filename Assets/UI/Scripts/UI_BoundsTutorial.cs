using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BoundsTutorial : MonoBehaviour {
    
    // protected GameObject[] textPrompts;
    float delay = 1.5f;

    [SerializeField] List<string> messages;
    [SerializeField] protected TMP_Text textLabel;

    public void DisplayTutorial(float textDelay) {
        delay = textDelay;

        StartCoroutine(ToggleThroughText());
    }

    protected IEnumerator ToggleThroughText() {
        
        textLabel.text = messages[0] + "\n\n \n\n ";
        yield return new WaitForSecondsRealtime(delay);

        textLabel.text = messages[0] + "\n\n" + messages[1] + "\n\n ";
        yield return new WaitForSecondsRealtime(delay);

        textLabel.text = messages[0] + "\n\n" + messages[1] + "\n\n" + messages[2];
        yield return new WaitForSecondsRealtime(delay);

    }
}
