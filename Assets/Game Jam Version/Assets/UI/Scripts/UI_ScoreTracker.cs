using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ScoreTracker : MonoBehaviour
{

    protected static UI_ScoreTracker _instance;
    public static UI_ScoreTracker Instance { get { return _instance; } }
    [SerializeField] protected TMP_Text scoreText;

    [Header("Score Config")]
    [SerializeField] protected HealthTracker playerHealth;
    [SerializeField] protected HealthTracker bossHealth;

    protected int passiveScoreValue = -100;

    int score;
    public int Score { get { return score; } }

    float scoreMult = 1.0f;

    float multMod = 0.06f;
    float multTime = 3f;

    void Start() {
        score = 0;

        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this.gameObject);
        }

        StartCoroutine(PassiveScoreIncrease());
    }

    void Update() {
        scoreText.text = "" + score;
    }

    protected IEnumerator PassiveScoreIncrease() {
        float timeToNextDrop = 0;
        while(playerHealth.CurrentHealth > 0 && bossHealth.CurrentHealth > 0) {
            timeToNextDrop -= Time.fixedDeltaTime;
            if (timeToNextDrop <= 0) {
                AddScore(passiveScoreValue, multiply:false);
                timeToNextDrop = 5f;
            }
            
            yield return new WaitForFixedUpdate();
        }

        if (playerHealth.CurrentHealth > 0) {
            Debug.Log("The player winned and earned " + (score * playerHealth.HealthPercentage) + " bompus points! ");
            AddScore( Mathf.RoundToInt(score * playerHealth.HealthPercentage) );
        }
    }

    public void AddScore(int scoreAmount, bool multiply = false) {
        score = (int) Mathf.Clamp(score + Mathf.RoundToInt((scoreAmount * scoreMult)), 0 , Mathf.Infinity);
        

        if (multiply) {
            StartCoroutine(ConsecutiveScoreMultiplier());
        }
    }

    protected IEnumerator ConsecutiveScoreMultiplier() {
        scoreMult += multMod;

        yield return new WaitForSeconds(multTime);

        scoreMult -= multMod;
    }
    

    
}
