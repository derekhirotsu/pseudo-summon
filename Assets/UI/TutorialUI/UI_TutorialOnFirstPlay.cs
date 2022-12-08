using System.Collections;
using UnityEngine;

namespace PseudoSummon.UI
{
    public class UI_TutorialOnFirstPlay : MonoBehaviour
    {
        [SerializeField]
        protected static UI_TutorialOnFirstPlay instance;
        public static UI_TutorialOnFirstPlay Instance { get { return instance; } }

        [SerializeField] protected bool firstPlay = true;
        public bool FirstPlay { get { return firstPlay; } }

        [Header("UI References")]
        [SerializeField] protected UI_MenuSwapper swapper;
        [SerializeField] protected UI_Countdown countdownMenu;
        [SerializeField] protected UI_BoundsTutorial boundsMenu;

        [Header("Player references")]
        protected PlayerController player;
        protected BossController boss;

        public System.Action TutorialEnded;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        public void StartTutorialSequence()
        {
            StartCoroutine(FirstPlayCoroutine());
        }

        protected IEnumerator FirstPlayCoroutine()
        {
            // yield return new WaitForSecondsRealtime(7.3f);

            // Display Bound UI
            swapper.ToggleMenu(2);
            boundsMenu.DisplayTutorial(5.5f / 3);
            yield return new WaitForSecondsRealtime(5.5f);

            // Display Coundown UI
            swapper.ToggleMenu(3);
            countdownMenu.StartCountdown(4, 2f / 4);
            yield return new WaitForSecondsRealtime(2.05f);

            swapper.ToggleMenus(0, 4);

            firstPlay = false;
            TutorialEnded?.Invoke();
        }
    }
}