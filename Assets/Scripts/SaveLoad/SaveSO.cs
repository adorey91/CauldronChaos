using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveFile", menuName = "New SaveFile")]
public class SaveSO : ScriptableObject
{
    public bool saveExists;
    public int unlockedDays;

    public int[] scoreDay;
    public int[] peopleServed;



    internal void Delete()
    {
        scoreDay = new int[5];  // Reset with correct length
        peopleServed = new int[5]; // Reset with correct length
        unlockedDays = 1;
        saveExists = false;
    }


    internal void SaveDayInfo(int day, int score, int people, int daysUnlcoked)
    {
        saveExists = true;
        scoreDay[day] = score;
        peopleServed[day] = people;
        unlockedDays = daysUnlcoked;
    }
}
