using System;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private LevelSelectionButtons[] levelSelection;

    private int unlockedDays;
    private int[] score;
    private int[] peopleServed;


    private void Start()
    {
        score = new int[levelSelection.Length];
        peopleServed = new int[levelSelection.Length];
    }

    private void OnEnable()
    {
        Actions.UpdateLevelButtons += UpdateButtons;
        Actions.OnSetUnlockedDays += SetUnlockedDays;
        Actions.OnSetScore += SetScore;
        Actions.OnSaveDeleted += ResetButtonLabels;
    }

    private void OnDisable()
    {
        Actions.UpdateLevelButtons -= UpdateButtons;
        Actions.OnSetUnlockedDays -= SetUnlockedDays;
        Actions.OnSetScore -= SetScore;
        Actions.OnSaveDeleted -= ResetButtonLabels;
    }

    private void ResetButtonLabels()
    {
        for(int i = 0; i < levelSelection.Length; i++)
        {
            levelSelection[i].buttonText.text = $"Day {i + 1}";
        }
    }


    private void UpdateButtons()
    {
        for (int i = 0; i < levelSelection.Length; i++)
        {
            if (levelSelection[i].button == null || levelSelection[i].dayImage == null || levelSelection[i].buttonText == null)
            {
                Debug.LogWarning($"LevelSelectionButtons[{i}] has a null reference.");
                continue; // Skip this iteration if any component is missing
            }

            if (i < unlockedDays)
            {
                levelSelection[i].button.interactable = true;

                // Check if dayImage is still valid before enabling it
                if (levelSelection[i].dayImage != null)
                {
                    levelSelection[i].dayImage.enabled = true;
                }

                if (score == null) break;
                if (GameManager.instance.isDebugging()) continue;
                if (score[i] == 0) continue;

                levelSelection[i].buttonText.text = $"Day {i + 1}\nScore: {score[i]}";
            }
            else
            {
                levelSelection[i].button.interactable = false;

                // Check if dayImage is still valid before disabling it
                if (levelSelection[i].dayImage != null)
                {
                    levelSelection[i].dayImage.enabled = false;
                }
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
}

[Serializable]
public class LevelSelectionButtons
{
    public Button button;
    public Image dayImage;
    public TextMeshProUGUI buttonText;
}
