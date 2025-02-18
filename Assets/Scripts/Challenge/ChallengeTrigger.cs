using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor.IMGUI.Controls;
using TMPro;
using UnityEditor;

public class ChallengeTrigger : MonoBehaviour
{
    public enum ChallengeType { None, SlipperyFloor, MovingCauldron, GoblinUnleashed, WindyDay, SnailSlimed }
    public ChallengeType challengeType;

    [SerializeField] private PhysicMaterial slipperyMaterial;
    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private TMP_Dropdown dropdown;


    public static Action OnStartChallenge;


    private void Start()
    {
        // used in testing scene only
        InitializeDropdown();
    }

    private void OnEnable()
    {
        OnStartChallenge += StartChallenge;
        Actions.OnEndDay += ResetChallenges;
    }

    private void OnDisable()
    {
        OnStartChallenge -= StartChallenge;
        Actions.OnEndDay -= ResetChallenges;
    }

    private void InitializeDropdown()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            foreach (ChallengeType challenge in Enum.GetValues(typeof(ChallengeType)))
            {
                string challengeName = ObjectNames.NicifyVariableName(challenge.ToString());
                dropdown.options.Add(new TMP_Dropdown.OptionData(challengeName));
            }

            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }
    private void OnDropdownChanged(int index)
    {
        challengeType = (ChallengeType)index; // Convert dropdown index to ChallengeType
        ResetChallenges();
        StartChallenge();
    }


    private void StartChallenge()
    {
        switch (challengeType)
        {
            case ChallengeType.None: ResetChallenges(); break;
            case ChallengeType.SlipperyFloor: 
                PlayerMovement.OnIceDay?.Invoke(true);
               Floor.OnApplyMaterial?.Invoke(slipperyMaterial);
                break;
            case ChallengeType.MovingCauldron: 
                CauldronMovement.OnStartChallenge?.Invoke (); 
                break;
            case ChallengeType.GoblinUnleashed: 
                GoblinAI.StartGoblinChaos?.Invoke(); 
                break;
            case ChallengeType.WindyDay: 
                GoblinAI.StartGoblinChaos?.Invoke(); 
                break;
            default: ResetChallenges(); break;
        }
    }

    private void ResetChallenges()
    {
        Floor.OnApplyMaterial?.Invoke(defaultMaterial);
        CauldronMovement.OnEndChallenge?.Invoke();
        PlayerMovement.OnIceDay?.Invoke(false);
        GoblinAI.EndGoblinChaos?.Invoke();
    }
}
