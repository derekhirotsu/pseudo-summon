using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PseudoSummon;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] protected CanvasGroup deathUI;
    [SerializeField] protected CanvasGroup pauseUI;
    [SerializeField] protected CanvasGroup optionsUI;
    [SerializeField] private GameObject _playerHud;
    [SerializeField] private GameOverMenu _gameOverMenu;

    [SerializeField] protected TMP_Text endTitle;
    [SerializeField] protected TMP_Text endMessage;
    [SerializeField] protected TMP_Text endButton;
    [SerializeField] protected TMP_Text finalScoreText;
    [SerializeField] protected TMP_Text scoreTipText;
    [SerializeField] protected TMP_Text pauseText;

    public void DisplayDeathUI(bool won = false)
    {
        _playerHud.SetActive(false);
        _gameOverMenu.gameObject.SetActive(true);


        if (won)
        {
            _gameOverMenu.DisplayMenuWin(GameManager.Instance.GetScore());
        }
        else
        {
            _gameOverMenu.DisplayMenuLose(GameManager.Instance.GetScore());
        }
    }

    public void DisplayPauseUI()
    {

        pauseText.text = "PAUSED";
        _playerHud.SetActive(false);
        pauseUI.gameObject.SetActive(true);
    }

    public void HidePauseUI()
    {
        _playerHud.SetActive(true);
        pauseUI.gameObject.SetActive(false);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitToMenu()
    {
        HideOptionsUI();
        HidePauseUI();
        LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayOptionsUI()
    {
        optionsUI.gameObject.SetActive(true);
    }

    public void HideOptionsUI()
    {
        optionsUI.gameObject.SetActive(false);
    }

}
