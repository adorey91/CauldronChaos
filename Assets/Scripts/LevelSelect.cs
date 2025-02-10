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


    private void OnEnable()
    {
        UpdateLevelButtons += UpdateButtons;
        SaveManager.SetUnlockedDays += SetUnlockedDays;
        SaveManager.SetScore += SetScore;
        SaveManager.SetPeopleServed += SetPeopleServed;
    }

    private void OnDisable()
    {
        UpdateLevelButtons -= UpdateButtons;
        SaveManager.SetUnlockedDays -= SetUnlockedDays;
        SaveManager.SetScore -= SetScore;
        SaveManager.SetPeopleServed -= SetPeopleServed;
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
                if (score[i] == 0) break;

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
    }

    public void SetScore(int[] score)
    {
        for(int i = 0; i < score.Length; i++)
        {
            this.score[i] = score[i];
        }
    }

    public void SetPeopleServed(int[] people)
    {
        for (int i = 0; i < peopleServed.Length; i++)
        {
            peopleServed[i] = people[i];
        }
    }
}
