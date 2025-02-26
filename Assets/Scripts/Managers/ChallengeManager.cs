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

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartChallenge += StartChallenge;
        Actions.OnResetChallenge += ResetChallenges;
    }

    private void OnDisable()
    {
        Actions.OnStartChallenge -= StartChallenge;
        Actions.OnResetChallenge -= ResetChallenges;
    }

    private void OnDestroy()
    {
        Actions.OnStartChallenge -= StartChallenge;
        Actions.OnResetChallenge -= ResetChallenges;
    }
    #endregion

    // Start the challenge based on the challenge type
    private void StartChallenge(int challenge)
    {
        Debug.Log("Challenge Started:   " + challenge);
        ResetChallenges();
        switch (challenge)
        {
            case 0: Debug.Log("No Challenges"); break;
            case 1: Actions.OnIceDay?.Invoke(true);
                Actions.OnApplyFoorMaterial?.Invoke(slipperyMaterial, icyTexture);
                break;
            case 2: Actions.OnStartCauldron?.Invoke(); break;
            case 3: Actions.OnStartGoblin?.Invoke(true); break;
            case 4: Actions.OnStartWindy?.Invoke(); break;
            case 5: Actions.OnStartSlime?.Invoke(); break;
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
        Actions.OnStopWindy?.Invoke();
    }
}