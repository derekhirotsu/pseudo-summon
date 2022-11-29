using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }

    void Start() {
        if (firstPlay) {
            StartCoroutine(FirstPlayCoroutine());
        }
    }

    protected IEnumerator FirstPlayCoroutine() {
        // yield return new WaitForSecondsRealtime(7.3f);

        // Display Bound UI
        swapper.ToggleMenu(4);
        boundsMenu.DisplayTutorial(5.5f /3);
        yield return new WaitForSecondsRealtime(5.5f);

        // Display Coundown UI
        swapper.ToggleMenu(5);
        countdownMenu.StartCountdown(4, 2f /4);
        yield return new WaitForSecondsRealtime(2.05f);

        swapper.ToggleMenu(1);

        firstPlay = false;
    }
}
