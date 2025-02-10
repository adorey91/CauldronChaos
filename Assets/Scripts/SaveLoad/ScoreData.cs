using UnityEngine;
using System;

[Serializable]
public class ScoreData
{
    // days unlocked
    public int unlockedDays;

    // score for each day
    public int[] scoreDay;

    // people served for each day
    public int[] peopleServed;

    public string[] whoOwnsScore;
}
