using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ChallengeManager : MonoBehaviour
{
    [Header("Floor Materials")]
    [SerializeField] private PhysicMaterial slipperyMaterial;
    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private Texture icyTexture;
    [SerializeField] private Texture defaultTexture;

    [Header("Dropdown used for Testing Scene Only")]
    [SerializeField] private TMP_Dropdown dropdown;
    private readonly List<string> challengeOptions = new()
    {
        "None",
        "Slippery Floor",
        "Moving Cauldron",
        "Goblin Unleashed",
        "Windy Day",
        "Slimey Trail"
    };

    private void Start()
    {
        // used in testing scene only
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            dropdown = GameObject.Find("Dropdown_Event").GetComponent<TMP_Dropdown>();
            InitializeDropdown();
        }
    }

    private void OnEnable()
    {
        Actions.OnStartChallenge += StartChallenge;
        Actions.OnSetDay += CheckChallengeDay;
        Actions.OnEndDay += ResetChallenges;
    }

    private void OnDisable()
    {
        Actions.OnStartChallenge -= StartChallenge;
        Actions.OnSetDay -= CheckChallengeDay;
        Actions.OnEndDay -= ResetChallenges;
    }
    #region Dropdown for Testing Scene
    // used in testing scene only, to change the challenge type
    private void InitializeDropdown()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(challengeOptions);
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }

    // used in testing scene only, to change the challenge type
    private void OnDropdownChanged(int index)
    {
        ResetChallenges();
        StartChallenge(index);
    }
    #endregion

    private void CheckChallengeDay(int currentDay)
    {
        //    Debug.Log("Current Day: " + currentDay);
        //    if (currentDay == 0) return;

        //    if ((currentDay) % 2 == 0)
        //    {
        //        Debug.Log("Challenge Day");
        //        Debug.Log(currentDay + 1);
        //        Actions.OnStartChallenge?.Invoke((currentDay + 1) / 2);
        //    }
        //    else
        //    {
        //        Debug.Log("Not a challenge day");
        //    }
        //    if ((currentDay + 1) > 5)
        //    {
        //        Actions.OnStartGoblin?.Invoke(false);
        //        Debug.Log("Goblin Chaos");
        //    }
    }


// Start the challenge based on the challenge type
private void StartChallenge(int challenge)
    {
        Debug.Log("Challenge Started:   " + challenge);
        ResetChallenges();
        switch (challenge)
        {
            case 0: Debug.Log("No Challenges"); break;
            case 1:
                Actions.OnIceDay?.Invoke(true);
                Actions.OnApplyFoorMaterial?.Invoke(slipperyMaterial, icyTexture);
                break;
            case 2:
                Actions.OnStartCauldron?.Invoke();
                break;
            case 3:
                Actions.OnStartGoblin?.Invoke(true);
                break;
            case 4:
                break;
            case 5:
                break;
            default: ResetChallenges(); break;
        }
    }

    // Reset all challenges
    private void ResetChallenges()
    {
        Actions.OnApplyFoorMaterial?.Invoke(defaultMaterial, defaultTexture);
        Actions.OnEndCauldron?.Invoke();
        Actions.OnIceDay?.Invoke(false);
        Actions.OnEndGoblin?.Invoke();
    }
}