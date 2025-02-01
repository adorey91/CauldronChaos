using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class SaveLoad : MonoBehaviour
{
    [SerializeField] private SaveSO saveFile;

    public static Action <int, int, int, bool> SaveInfo;
    public static Action<bool> OnSaveExist;

    private int unlockedDays;

    private void Start()
    {
        if(saveFile == null)
        {
            saveFile = ScriptableObject.CreateInstance<SaveSO>();
        }
    }

    private void OnEnable()
    {
        SaveInfo += SaveInformation;
    }

    private void OnDisable()
    {
        SaveInfo -= SaveInformation;
    }

    public void DeleteSave()
    {
        saveFile.Delete();
    }

    public void SaveInformation(int day, int score, int people, bool nextDay)
    {
        if (nextDay)
        {
            unlockedDays = day + 1;
        }
        saveFile.SaveDayInfo(day, score, people, unlockedDays);
    }

    public int CheckUnlockedDays()
    {
        return saveFile.unlockedDays;
    }

    public int CheckScore(int day)
    {
        return saveFile.scoreDay[day];
    }

    public void CheckForSave()
    {
        if (saveFile.saveExists)
        {
            OnSaveExist?.Invoke(true);
        }
        else
        {
            OnSaveExist?.Invoke(false);
        }
    }
}
