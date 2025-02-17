using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public bool isValidSave;
    public List<DayData> days = new(10);

    public void CreateNewSave()
    {
        isValidSave = false;
        days.Clear();

        for (int i = 0; i < 10; i++)
        {
            days.Add(new DayData());
            days[i].dayNumber = i + 1;
        }
    }
}


[Serializable]
public class DayData
{
    public int dayNumber;
    public bool isUnlocked;
    public int bestScore;
    public int peopleServed;

    public DayData(bool unlocked = false, int score = 0, int people = 0)
    {
        isUnlocked = unlocked;
        bestScore = score;
        peopleServed = people;
    }
}


