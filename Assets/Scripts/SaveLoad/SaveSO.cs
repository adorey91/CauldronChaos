using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveFile", menuName = "New SaveFile")]
public class SaveSO : ScriptableObject
{
    // days unlocked
    public int unlockedDays;

    public bool savedScores;

    // score for each day
    public int[] scoreDay;

    // people served for each day
    public int[] peopleServed;

    public string[] whoOwnsScore;
}
