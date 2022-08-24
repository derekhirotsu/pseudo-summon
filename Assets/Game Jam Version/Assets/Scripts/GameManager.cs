using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] protected PlayerCanvas UI;
        [SerializeField] private CameraHolder camHolder;
        
        private PlayerController playerController;
        private BossController bossController;


        private void Start()
        {
            FindBoss();
            FindPlayer();

            playerController.Died += PlayerController_Died;

            bossController.DamageTaken += BossController_DamageTaken;
            bossController.Died += BossController_Died;
        }

        private void Update()
        {
            HandlePauseInput();
        }

        private void FindPlayer()
        {
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        }

        private void FindBoss()
        {
            bossController = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<BossController>();
        }


        private void PlayerController_Died(object sender, System.EventArgs e)
        {
            UI.DisplayDeathUI(won: false);
        }

        private void BossController_Died(object sender, System.EventArgs e)
        {
            playerController.CanDie = false;
            playerController.enabled = false;
            UI.DisplayDeathUI(won: true);
        }

        private void BossController_DamageTaken(object sender, System.EventArgs e)
        {
            playerController.OnBossHit();
        }

        private void HandlePauseInput()
        {
            if (Input.GetButtonDown("Pause"))
            {
                if (UI_TutorialOnFirstPlay.Instance.FirstPlay || playerController.IsDead)
                {
                    return;
                }

                if (playerController.IsPaused)
                {
                    Unpause();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void Pause()
        {
            camHolder.CancelHitStop();

            Music.instance.SetLowPassFilterEnabled(true);
            Time.timeScale = 0f;
            playerController.IsPaused = true;
            UI.DisplayPauseUI();
        }

        public void Unpause()
        {
            Music.instance.SetLowPassFilterEnabled(false);
            playerController.IsPaused = false;
            Time.timeScale = 1f;
            UI.HidePauseUI();
            UI.HideOptionsUI();
        }
    }
}
