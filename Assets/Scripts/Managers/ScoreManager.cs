using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI peopleServedText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int daysToPlay = 5;

    [Header("EOD UI")]
    [SerializeField] private TextMeshProUGUI eodTitle;
    [SerializeField] private TextMeshProUGUI peopleServedEOD;
    [SerializeField] private TextMeshProUGUI eodScoreText;

    [Header("Score Amounts")]
    [SerializeField] private int goodPotionScore = 10;
    [SerializeField] private int badPotionScore = 5;
    [SerializeField] private int noOrderServedScore = -8;

    private int _score = 0;
    private int _people = 0;
    private int _currentDay = 0;

    private void Start()
    {
        _currentDay = 0;
        _score = 0;
        _people = 0;

        dayText.text = $"Day: {_currentDay}/{daysToPlay}";
        scoreText.text = $"Score: {_score}";
        peopleServedText.text = $"People Served: {_people}";
    }

    private void OnEnable()
    {
        Actions.OnCustomerServed += UpdateScore;
        Actions.OnNoCustomerServed += UpdateNoPersonServed;
        Actions.OnStartDay += RestartDay;
        Actions.OnEndDay += UpdateEODText;
    }

    private void OnDisable()
    {
        Actions.OnCustomerServed -= UpdateScore;
        Actions.OnNoCustomerServed -= UpdateNoPersonServed;
        Actions.OnStartDay -= RestartDay;
        Actions.OnEndDay -= UpdateEODText;
    }

    private void UpdateScore(bool wasPotionGood)
    {
        if (wasPotionGood)
            _score += goodPotionScore;
        else
            _score += badPotionScore;
         
        _people++;

        scoreText.text = "Score: " + _score;
        peopleServedText.text = "People Served: " + _people;
    }

    private void UpdateNoPersonServed()
    {
        _score += noOrderServedScore;
        scoreText.text = "Score: " + _score;
    }

    private void UpdateEODText()
    {
        eodTitle.text = $"End of Day {_currentDay}";
        peopleServedEOD.text = $"People Served: {_people}";
        eodScoreText.text = $"Score: {_score}";
    }

    private void RestartDay()
    {
        _currentDay++;
        _score = 0;
        _people = 0;

        dayText.text = $"Day: {_currentDay}/{daysToPlay}";
        scoreText.text = $"Score: {_score}";
        peopleServedText.text = $"People Served: {_people}";
    }

}
