using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Countdown : MonoBehaviour
{
    [SerializeField] protected TMP_Text countdownText;
    [SerializeField] protected bool countdownInProgress = false;
    public bool CountdownInProgress { get { return countdownInProgress; } }

    protected float curTicks = 3;
    protected float curDelayBetweenTicks = 0.8f;

    public void StartCountdown(int ticks, float delayBetweenTicks) {

        if (countdownInProgress) { 
            return;
        }

        curTicks = ticks;
        curDelayBetweenTicks = delayBetweenTicks;

        StartCoroutine(Countdown());
    }

    protected IEnumerator Countdown() {
        countdownInProgress = true;

        while (curTicks > 1) {
            countdownText.text = "" + (--curTicks);
            yield return new WaitForSecondsRealtime(curDelayBetweenTicks);
        }

        countdownText.text = "Go!";
        yield return new WaitForSecondsRealtime(curDelayBetweenTicks);

        countdownInProgress = false;
    }
}
