using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private LevelSelectionButtons[] levelSelection;

    private int unlockedDays;
    private int[] score;
    private int[] peopleServed;

    public static Action UpdateLevelButtons;
    public static Action<int> OnSetUnlockedDays;
    public static Action<int[]> OnSetScore;
    public static Action<int[]> OnSetPeopleServed;

    private void Start()
    {
        score = new int[levelSelection.Length];
        peopleServed = new int[levelSelection.Length];
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
        for (int i = 0; i < levelSelection.Length; i++)
        {
            if (i < unlockedDays)
            {
                levelSelection[i].button.interactable = true;
                levelSelection[i].dayImage.enabled = true;

                if (score == null) break;
                if (score[i] == 0) continue;

                levelSelection[i].buttonText.text = $"Day {i + 1}\nScore: {score[i]}";
            }
            else
            {
                levelSelection[i].button.interactable = false;
                levelSelection[i].dayImage.enabled = false;
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

[Serializable]
public class LevelSelectionButtons
{
    public Button button;
    public Image dayImage;
    public TextMeshProUGUI buttonText;
}
