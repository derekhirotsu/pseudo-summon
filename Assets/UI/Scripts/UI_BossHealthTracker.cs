using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHealthTracker : MonoBehaviour
{
    [SerializeField] protected HealthTracker bossHealth;
    [SerializeField] Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        if (bossHealth == null) {
            this.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        slider.value = bossHealth.HealthPercentage;
    }
}
