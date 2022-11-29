using UnityEngine;
using UnityEngine.SceneManagement;

namespace PseudoSummon.UI
{
    public class TitleScreenMenu : MonoBehaviour
    {
        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}