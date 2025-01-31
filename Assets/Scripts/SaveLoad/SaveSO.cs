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
        saveExists = false;
        for (int i =0; i < scoreDay.Length; i++)
        {
            scoreDay[i] = 0;
            peopleServed[i] = 0;
        }

        unlockedDays = 1;
    }

    internal void SaveDayInfo(int day, int score, int people, int daysUnlcoked)
    {
        saveExists = true;
        scoreDay[day] = score;
        peopleServed[day] = people;
        unlockedDays = daysUnlcoked;
    }
}
