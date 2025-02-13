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

    [SerializeField] private GameObject[] floor;
    [SerializeField] private GameObject[] cauldron;
    [SerializeField] private GameObject goblin;

    [SerializeField] private PhysicMaterial slipperyMaterial;
    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private TMP_Dropdown dropdown;


    private void Start()
    {
        if (floor.Length > 0)
            defaultMaterial = floor[0].GetComponent<Collider>().material;

        InitializeDropdown();
        StartChallenge();
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
                foreach (GameObject floor in floor)
                {
                    floor.GetComponent<Collider>().material = slipperyMaterial;
                }
                break;
            case ChallengeType.MovingCauldron: CauldronMovement.OnStartChallenge?.Invoke (); break;
            case ChallengeType.GoblinUnleashed: break;
            case ChallengeType.WindyDay: break;
            default: ResetChallenges(); break;
        }
    }

    private void ResetChallenges()
    {
        foreach (GameObject floor in floor)
        {
            floor.GetComponent<Collider>().material = defaultMaterial;
        }

        CauldronMovement.OnEndChallenge?.Invoke();
        goblin.GetComponent<GoblinAI>().enabled = false;
        PlayerMovement.OnIceDay?.Invoke(false);
    }
}
