using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthTracker : MonoBehaviour
{
    protected HealthTracker tracker;
    [SerializeField] Text trackerText;
    [SerializeField] protected string trackerMessage = "Health: ";

    void Start() {
        // trackerText = this.GetComponent<Text>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0) {
            tracker = players[0].GetComponent<HealthTracker>();
        }
        
        if (tracker == null) {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tracker.CurrentHealth <= 0) {
            this.gameObject.SetActive(false);
        }

        trackerText.text = trackerMessage + tracker.CurrentHealth;

    }
}
