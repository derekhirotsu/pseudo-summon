using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] protected CanvasGroup deathUI;
    [SerializeField] protected CanvasGroup pauseUI;
    [SerializeField] protected CanvasGroup optionsUI;
    [SerializeField] protected CanvasGroup mainUI;

    [SerializeField] protected TMP_Text endTitle;
    [SerializeField] protected TMP_Text endMessage;
    [SerializeField] protected TMP_Text endButton;
    [SerializeField] protected TMP_Text finalScoreText;
    [SerializeField] protected TMP_Text scoreTipText;
    [SerializeField] protected TMP_Text pauseText;
    
    public void DisplayDeathUI(bool won = false) {

        mainUI.alpha = 0f;

        if (won) {
            // Win text
            endTitle.text = "Vanquished!";
            endMessage.text = "Your Resurrection was Achieved";
            endButton.text = "Once More!";

            scoreTipText.gameObject.SetActive(true);

        } else {
            // Lose text
            endTitle.text = "Banished...";
            endMessage.text = "Your Resurrection was Denied";
            endButton.text = "Retry";
        }

        finalScoreText.text = "Final Score : - " + UI_ScoreTracker.Instance.Score + " - ";
        
        deathUI.gameObject.SetActive(true);
    }

    public void DisplayPauseUI() {

        pauseText.text = "PAUSED";
        mainUI.alpha = 0f;
        pauseUI.gameObject.SetActive(true);
    }

    public void UpdatePauseUI() {
        pauseText.text = "Resuming...";
    }

    public void HidePauseUI() {
        mainUI.alpha = 1f;
        pauseUI.gameObject.SetActive(false);
    }

    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void QuitToMenu() {
        HideOptionsUI();
        HidePauseUI();
        LoadScene("StartScene");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void DisplayOptionsUI() {
        // HidePauseUI();
        optionsUI.gameObject.SetActive(true);
    }

    public void HideOptionsUI() {
        optionsUI.gameObject.SetActive(false);
        // DisplayPauseUI();
    }

}
