using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public bool isValidSave;
    public List<DayData> days = new();
}


[Serializable]
public class DayData
{
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


