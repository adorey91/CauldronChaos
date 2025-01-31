using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveFile", menuName = "New SaveFile")]
public class SaveSO : ScriptableObject
{
    public int unlockedDays;

    public int[] scoreDay;
    public int[] peopleServed;


    internal void Delete()
    {
        for(int i =0; i < scoreDay.Length; i++)
        {
            scoreDay[i] = 0;
            peopleServed[i] = 0;
        }

        unlockedDays = 0;
    }

    internal void SaveDayInfo(int day, int score, int people)
    {
        scoreDay[day] = score;
        peopleServed[day] = people;
    }
}
