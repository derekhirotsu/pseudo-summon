using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] protected PlayerCanvas UI;

        private PlayerController playerController;
        private BossController bossController;

        private void FindPlayer()
        {
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        }

        private void FindBoss()
        {
            bossController = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<BossController>();
        }

        private void Start()
        {
            FindBoss();
            FindPlayer();

            bossController.DamageTaken += BossController_DamageTaken;
            bossController.Died += BossController_Died;
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
    }
}
