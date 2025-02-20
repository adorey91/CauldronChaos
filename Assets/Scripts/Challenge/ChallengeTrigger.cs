using UnityEngine;
using TMPro;

public class ChallengeTrigger : MonoBehaviour
{
    public enum ChallengeType { None, SlipperyFloor, MovingCauldron, GoblinUnleashed, WindyDay, SnailSlimed }
    public ChallengeType challengeType;

    [Header("Floor Materials")]
    [SerializeField] private PhysicMaterial slipperyMaterial;
    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private Texture icyTexture;
    [SerializeField] private Texture defaultTexture;

    [Header("Dropdown used for Testing Scene Only")]
    [SerializeField] private TMP_Dropdown dropdown;

    /// <summary> Event to start the challenge </summary>
    public static System.Action OnStartChallenge;

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

    // used in testing scene only, to change the challenge type
    private void InitializeDropdown()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            foreach (ChallengeType challenge in System.Enum.GetValues(typeof(ChallengeType)))
            {
                string challengeName = challenge.ToString();
                dropdown.options.Add(new TMP_Dropdown.OptionData(challengeName));
            }

            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }

    // used in testing scene only, to change the challenge type
    private void OnDropdownChanged(int index)
    {
        challengeType = (ChallengeType)index; // Convert dropdown index to ChallengeType
        ResetChallenges();
        StartChallenge();
    }

    // Start the challenge based on the challenge type
    private void StartChallenge()
    {
        switch (challengeType)
        {
            case ChallengeType.None: ResetChallenges(); break;
            case ChallengeType.SlipperyFloor: 
                PlayerMovement.OnIceDay?.Invoke(true);
               Floor.OnApplyMaterial?.Invoke(slipperyMaterial, icyTexture);
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

    // Reset all challenges
    private void ResetChallenges()
    {
        Floor.OnApplyMaterial?.Invoke(defaultMaterial, defaultTexture);
        CauldronMovement.OnEndChallenge?.Invoke();
        PlayerMovement.OnIceDay?.Invoke(false);
        GoblinAI.EndGoblinChaos?.Invoke();
    }
}
