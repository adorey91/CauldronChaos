using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TestChallenge : MonoBehaviour
{
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

    [Header("Toggle")]
    [SerializeField] private Toggle windLeft;
    [SerializeField] private Toggle windRight;
    [SerializeField] private Toggle windTowards;
    [SerializeField] private Toggle windCircle;

    // used in testing scene only
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            dropdown = GameObject.Find("Dropdown_Event").GetComponent<TMP_Dropdown>();
            InitializeDropdown();
        }
    }


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
        Actions.OnStartChallenge?.Invoke(index);
    }
}
