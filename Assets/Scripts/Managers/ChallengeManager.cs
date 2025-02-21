using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// <summary> Event to start the challenge </summary>
    public static System.Action<int> OnStartChallenge;
    public static System.Action CheckChallengeDay;

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
        OnStartChallenge += StartChallenge;
        Actions.OnEndDay += ResetChallenges;
    }

    private void OnDisable()
    {
        OnStartChallenge -= StartChallenge;
        Actions.OnEndDay -= ResetChallenges;
    }

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

    // Start the challenge based on the challenge type
    private void StartChallenge(int challenge)
    {
        

        switch (challenge)
        {
            case 0: ResetChallenges(); break;
            case 1:
                PlayerMovement.OnIceDay?.Invoke(true);
                Floor.OnApplyMaterial?.Invoke(slipperyMaterial, icyTexture);
                break;
            case 2:
                CauldronMovement.OnStartChallenge?.Invoke();
                break;
            case 3:
                GoblinAI.StartGoblinChaos?.Invoke(true);
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
        Floor.OnApplyMaterial?.Invoke(defaultMaterial, defaultTexture);
        CauldronMovement.OnEndChallenge?.Invoke();
        PlayerMovement.OnIceDay?.Invoke(false);
        GoblinAI.EndGoblinChaos?.Invoke();
    }
}