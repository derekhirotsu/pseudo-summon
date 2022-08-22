using PseudoSummon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SuperMeter : MonoBehaviour {
    [SerializeField] protected PlayerController player;

    [SerializeField] protected Image circleFill;
    [SerializeField] protected Image mouseIcon;
    [SerializeField] protected Image mouseFill;


    void Update() {
        if (!player.gameObject.activeSelf) {
            this.enabled = false;
        }

        circleFill.fillAmount = player.BusterFillPercent;

        if (player.BusterCharged() && mouseFill.IsActive()) {
            mouseFill.gameObject.SetActive(false);
        } else if (!player.BusterCharged() && !mouseFill.IsActive()) {
            mouseFill.gameObject.SetActive(true);
        }
    }
    
}
