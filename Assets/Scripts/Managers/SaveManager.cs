using System;
using System.IO;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    private string savePath;
    private string saveFileName = "cauldronChaos.json";

    public GameData gameData;

    public static Action<bool> OnSaveExist;
    public static Action<int, int, int, bool> OnSaveDay;

    public void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        LoadGame();
    }

    private void OnEnable()
    {
        OnSaveDay += SaveDayScore;
    }

    private void OnDisable()
    {
        OnSaveDay -= SaveDayScore;
    }

    public void CheckSaveFile()
    {
        if (gameData.isValidSave)
        {
            LevelSelect.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
            LevelSelect.OnSetScore?.Invoke(GetAllScores());
            LevelSelect.OnSetPeopleServed?.Invoke(GetAllPeopleServed());
            OnSaveExist?.Invoke(true);
        }
        else
        {
            OnSaveExist?.Invoke(false);
            LevelSelect.OnSetUnlockedDays?.Invoke(1);
            gameData = new GameData();
            gameData.CreateNewSave();
        }
    }


    public void SaveGame()
    {
        gameData.isValidSave = true;

        LevelSelect.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
        LevelSelect.OnSetScore?.Invoke(GetAllScores());
        LevelSelect.OnSetPeopleServed?.Invoke(GetAllPeopleServed());

        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(savePath, json);
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

    private void SaveDayScore(int day, int score, int people, bool unlockNext)
    {
        Debug.Log("saving day score " + day + " Is next day unlocked? " + unlockNext);
        // Update the day data if the day is valid
        if (day < gameData.days.Count)
        {
            DayData dayData = gameData.days[day];

            // Update the best score and people served for the day
            if (score > dayData.bestScore)
                dayData.bestScore = score;

            if (people > dayData.peopleServed)
                dayData.peopleServed = people;

            // Unlock the next day if the current day is completed
            if (unlockNext)
                UnlockDay(day+1 );
        }

        SaveGame();
    }

    private void UnlockDay(int day)
    {
        if (day < gameData.days.Count)
        {
            gameData.days[0].isUnlocked = true;
            gameData.days[day].isUnlocked = true;
            LevelSelect.OnSetUnlockedDays?.Invoke(GetUnlockedDaysCount());
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

    private int[] GetAllPeopleServed()
    {
        int[] people = new int[gameData.days.Count];
        for (int i = 0; i < gameData.days.Count; i++)
        {
            people[i] = gameData.days[i].peopleServed;
        }
        return people;
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");

            if (!File.Exists(savePath))
            {
                Debug.Log("Save file successfully deleted.");
                gameData = new GameData();
                gameData.CreateNewSave();

            }
            else
            {
                Debug.LogError("Failed to delete save file.");
            }
        }
    }
}
