using UnityEngine;
using UnityEngine.SceneManagement;

namespace PseudoSummon.UI
{
    public class PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private GameOverMenu _gameOverMenu;
        
        private UI_MenuSwapper _menuSwapper;

        private void Awake()
        {
            _menuSwapper = GetComponent<UI_MenuSwapper>();
        }

        public void DisplayDeathUI(bool won = false)
        {
            _menuSwapper.ToggleMenu(5);

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
            _menuSwapper.ToggleMenu(6);
        }

        public void HidePauseUI()
        {
            _menuSwapper.ToggleMenus(0, 4);
        }

        public void DisplayOptionsUI()
        {
            _menuSwapper.ToggleMenu(1);
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}