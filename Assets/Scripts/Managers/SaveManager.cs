using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SaveManager : MonoBehaviour
{
    private string savePath;
    private string saveFileName = "cauldronChaos.json";
    [SerializeField] private TextMeshProUGUI deleteFileConfirmation;

    public GameData gameData;

    private Coroutine deleteConfirm;

    public void Start()
    {
       deleteFileConfirmation.enabled = false;
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        LoadGame();
    }

    private void OnEnable()
    {
        Actions.OnSaveDay += SaveDayScore;
        Actions.OnDeleteSaveFile += DeleteSave;
    }

    private void OnDisable()
    {
        Actions.OnSaveDay -= SaveDayScore;
        Actions.OnDeleteSaveFile -= DeleteSave;
    }

    public void CheckSaveFile()
    {
        if (gameData.isValidSave && !GameManager.instance.isDebugging())
        {
            Actions.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
            Actions.OnSetScore?.Invoke(GetAllScores());
            Actions.OnSaveExist?.Invoke(true);
        }
        else
        {
            if(GameManager.instance.isDebugging())
            {
                Actions.OnSaveExist?.Invoke(true);
                Actions.OnSetUnlockedDays?.Invoke(10);
                return;
            }
            else
            {
                Actions.OnSaveExist?.Invoke(false);
                Actions.OnSetUnlockedDays?.Invoke(1);
                Actions.OnSetDay(1);
                gameData = new GameData();
                gameData.CreateNewSave();
            }
        }
    }


    public void SaveGame()
    {
        if(!GameManager.instance.isDebugging())
        {
            gameData.isValidSave = true;

            Actions.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
            Actions.OnSetScore?.Invoke(GetAllScores());

            string json = JsonUtility.ToJson(gameData);
            File.WriteAllText(savePath, json);
        }
        else
        {
            Debug.Log("Save file not created in debug mode");
        }
    }

    private void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);

            gameData.days[0].isUnlocked = true;
        }
    }

    public void SaveAbovePlayerUI(Toggle abovePlayer)
    {
        if (!GameManager.instance.isDebugging())
        {
            gameData.abovePlayerUI = abovePlayer.isOn;

            string json = JsonUtility.ToJson(abovePlayer);
            File.WriteAllText(savePath, json);
        }
        else
        {
            Debug.Log("Save file not created in debug mode");
        }
    }

    private void SaveDayScore(int day, int score, bool unlockNext)
    {
        if (!GameManager.instance.isDebugging())
        {
            // Update the day data if the day is valid
            if (day < gameData.days.Count)
            {
                DayData dayData = gameData.days[day];

                // Update the best score and people served for the day
                if (score > dayData.bestScore)
                    dayData.bestScore = score;

                // Unlock the next day if the current day is completed
                if (unlockNext)
                    UnlockDay(day + 1);
            }

            SaveGame();
        }
        else
        {
            Debug.Log("Save file not created in debug mode");
        }
    }

    private void UnlockDay(int day)
    {
        if (day < gameData.days.Count)
        {
            gameData.days[0].isUnlocked = true;
            gameData.days[day].isUnlocked = true;
            Actions.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
        }
    }


    private int GetUnlockedDaysCount()
    {
        int count = 0;
        foreach (DayData day in gameData.days)
        {
            if (day.isUnlocked)
                count++;
        }
        
        return count;
    }

    private int[] GetAllScores()
    {
        int[] scores = new int[gameData.days.Count];
        for (int i = 0; i < gameData.days.Count; i++)
        {
            scores[i] = gameData.days[i].bestScore;
        }
        return scores;
    }

    private void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);

            // Will reset the game data and refresh the level select screen
            if (!File.Exists(savePath))
            {
                gameData = new GameData();
                gameData.CreateNewSave();
                Actions.OnSaveDeleted?.Invoke();
                deleteConfirm = StartCoroutine(ShowDeleteConfirmation("Save File Deleted"));
            }
        }
        else
        {
            deleteConfirm = StartCoroutine(ShowDeleteConfirmation("No Save File Found"));
        }
    }

    private IEnumerator ShowDeleteConfirmation(string text)
    {
        deleteFileConfirmation.enabled = true;
        deleteFileConfirmation.text = text;
        yield return new WaitForSeconds(2);
        deleteFileConfirmation.enabled = false;
    }
}
