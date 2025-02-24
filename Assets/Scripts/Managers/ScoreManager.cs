using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    [Header("Gameplay UI")]
    [SerializeField] private Image quotaFill;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private ParticleSystem coinParticles;

    [Header("EOD UI")]
    [SerializeField] private TextMeshProUGUI eodTitle;
    [SerializeField] private TextMeshProUGUI eodScoreText;

    [Header("EOD Win Lose Sprite")]
    [SerializeField] private Image eodWinLoseSprite;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("Score Amounts")]
    [SerializeField] private int tipMultiplier = 2;
    [SerializeField] private int[] scorePerLevel;

    // keeps track of current day / day score
    private int score = 0;
    private int currentDay = 0;

    private void Start()
    {
        quotaFill.fillAmount = 0;
        coinParticles.Stop();
    }

    private void OnEnable()
    {
        Actions.OnCustomerServed += UpdateScore;
        Actions.OnEndDay += UpdateEODText;
        Actions.OnResetValues += ResetValues;
        Actions.OnSetDay += SetCurrentDay;
    }

    private void OnDisable()
    {
        Actions.OnCustomerServed -= UpdateScore;
        Actions.OnEndDay -= UpdateEODText;
        Actions.OnResetValues -= ResetValues;
        Actions.OnSetDay -= SetCurrentDay;
    }

    private void SetCurrentDay(int day)
    {
        currentDay = day;
    }

    private void UpdateScore(bool wasGivenOnTime, int regularScore)
    {
        int _addToTotalScore;
        if (wasGivenOnTime)
        {
            int addedScore = regularScore * tipMultiplier;
            score += addedScore;
            _addToTotalScore = addedScore;
        }
        else
        {
            score += regularScore;
            _addToTotalScore = regularScore;
        }

        if(score > scorePerLevel[currentDay] && !coinParticles.isPlaying)
        {
            coinParticles.Play();
        }

        quotaFill.fillAmount = (float)score / (float)scorePerLevel[currentDay];
        coinImage.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 0.5f);
    }

    private void UpdateEODText()
    {
        bool increaseDayCount;

        // Check if the player has reached the score for the current day
        if (score < scorePerLevel[currentDay])
            increaseDayCount = false;
        else
            increaseDayCount = true;

        // Save the current day's score and people served
        Actions.OnSaveDay(currentDay, score, increaseDayCount);

        // Sets the EOD text
        eodTitle.text = $"End of Day {currentDay + 1}";
        //peopleServedEOD.text = $"People Served: {people}";

        // Sets the EOD win/lose sprite and score text
        if (!increaseDayCount)
        {
            eodScoreText.color = Color.red;
            eodWinLoseSprite.sprite = loseSprite;
            eodScoreText.text = $"Score: {score}\nTry Level Again";
        }
        else
        {
            eodScoreText.color = Color.green;
            eodWinLoseSprite.sprite = winSprite;
            eodScoreText.text = $"Score: {score}";

        }

        score = 0;
    }


    public void ResetValues()
    {
        coinParticles.Stop();
        quotaFill.fillAmount = 0;
        score = 0;
    }
}
