using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Header("LevelButtons")]
    [SerializeField] private Button[] levelButtons;

    private int unlockedDays;
    private int[] score;
    private int[] peopleServed;

    public static Action UpdateLevelButtons;
    public static Action<int> OnSetUnlockedDays;
    public static Action<int[]> OnSetScore;
    public static Action<int[]> OnSetPeopleServed;

    private void Start()
    {
        score = new int[levelButtons.Length];
        peopleServed = new int[levelButtons.Length];
    }

    private void OnEnable()
    {
        UpdateLevelButtons += UpdateButtons;
        OnSetUnlockedDays += SetUnlockedDays;
        OnSetScore += SetScore;
        OnSetPeopleServed += SetPeopleServed;
    }

    private void OnDisable()
    {
        UpdateLevelButtons -= UpdateButtons;
        OnSetUnlockedDays += SetUnlockedDays;
        OnSetScore += SetScore;
        OnSetPeopleServed += SetPeopleServed;
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (i < unlockedDays)
            {
                levelButtons[i].interactable = true;

                if (score == null) break;
                if (score[i] == 0) continue;

                buttonText.text = $"Day {i + 1}\nScore: {score[i]}";
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }


    public void SetUnlockedDays(int days)
    {
        unlockedDays = days;

        UpdateButtons();
    }

    public void SetScore(int[] _score)
    {
        score = _score;
    }

    public void SetPeopleServed(int[] _people)
    {
        peopleServed = _people;
    }
}
