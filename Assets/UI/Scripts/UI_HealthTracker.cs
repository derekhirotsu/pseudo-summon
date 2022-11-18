using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthTracker : MonoBehaviour
{
    protected HealthTracker tracker;
    [SerializeField] private Text trackerText;
    [SerializeField] protected string trackerMessage = "Health: ";

    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            tracker = players[0].GetComponent<HealthTracker>();
        }

        if (tracker == null)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (tracker.CurrentHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }

        trackerText.text = trackerMessage + tracker.CurrentHealth;

    }
}
