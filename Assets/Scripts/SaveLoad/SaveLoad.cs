using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class SaveLoad : MonoBehaviour
{
    [SerializeField] private SaveSO saveFile;

    public static Action <int, int, int> SaveInfo;

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

    public void SaveInformation(int day, int score, int people)
    {
        saveFile.SaveDayInfo(day, score, people);
    }
}
