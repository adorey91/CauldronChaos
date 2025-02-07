using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string savePath;
    private string saveFileName = "cauldronChaos.json";
    [SerializeField] private SaveSO backup;

    public static Action<int, int, int, bool> SaveInfo;
    public static Action<bool> OnSaveExist;
    public static Action<int> SetUnlockedDays;
    public static Action<int[]> SetScore;
    public static Action<int[]> SetPeopleServed;

    public void Start()
    {
        savePath = Path.Combine(GetSavePath(), saveFileName);
    }

    private void OnEnable()
    {
        SaveInfo += SaveInformation;
    }

    private void OnDisable()
    {
        SaveInfo -= SaveInformation;
    }

    #region Save
    private void SaveInformation(int day, int score, int people, bool nextDay)
    {
        ScoreData data = new();

        if (nextDay)
        {
            data.unlockedDays = day + 1;
            backup.unlockedDays = day + 1;
        }
        else
        {
            data.unlockedDays = day;
            backup.unlockedDays = day;
        }

        data.scoreDay[day] = score;
        data.peopleServed[day] = people;

        backup.scoreDay[day] = score;
        backup.peopleServed[day] = people;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void CheckForSave()
    {
        if (File.Exists(savePath))
        {
            OnSaveExist?.Invoke(true);
            LoadInformation();
        }
        else
            OnSaveExist?.Invoke(false);
    }
    #endregion

    #region Load
    private void LoadInformation()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                ScoreData data = JsonUtility.FromJson<ScoreData>(json);

                SetUnlockedDays?.Invoke(data.unlockedDays);
                SetScore?.Invoke(data.scoreDay);
                SetPeopleServed?.Invoke(data.peopleServed);

                backup.unlockedDays = data.unlockedDays;
                backup.scoreDay = data.scoreDay;
                backup.peopleServed = data.peopleServed;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load save data: " + e.Message);
                // Optionally delete corrupted save
                File.Delete(savePath);
            }
        }
    }

    #endregion

    private string GetSavePath()
    {
        string path = Application.persistentDataPath;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");
        }
    }
}
