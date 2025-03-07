using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    [SerializeField] private string debugPassword = "Studio_CLAMS";
    
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Toggle debugToggle;
    [SerializeField] private Toggle debugMenuToggle;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TextMeshProUGUI debugTitle;
    
    private bool isManuallyToggling = false; // Prevents unwanted function calls

    private Coroutine textCoroutine;

    private void Start()
    {
        passwordInput.onEndEdit.AddListener(delegate { CheckPassword(); });
        backButton.onClick.AddListener(BackButton);


        debugToggle.isOn = false;
        debugMenuToggle.isOn = false;
        debugToggle.gameObject.SetActive(false);
        debugText.enabled = false;

        debugMenuToggle.onValueChanged.AddListener(delegate { ToggleMenuDebugMode(); });
        debugToggle.onValueChanged.AddListener(delegate { ToggleDebugMode(); });
    }

    private void CheckPassword()
    {
        if (passwordInput.text == debugPassword)
        {
            if(textCoroutine != null)
                StopCoroutine(textCoroutine);

            debugTitle.text = "Enter/Exit Debug Mode";
            textCoroutine = StartCoroutine(VisualText("Debug Mode Unlocked"));

            debugToggle.gameObject.SetActive(true);
            // Clear password input field if the password is incorrect
            passwordInput.text = "";
            passwordInput.gameObject.SetActive(false);
        }
        else
        {
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            textCoroutine = StartCoroutine(VisualText("Incorrect Password"));
            // Clear password input field if the password is incorrect
            passwordInput.text = "";
        }
    }

    private void BackButton()
    {
        debugToggle.gameObject.SetActive(false);
        passwordInput.gameObject.SetActive(true);
        debugTitle.text = "Enter Debug Password";
        Actions.OnOpenSettingsAction?.Invoke();
    }

    public void ToggleDebugMode()
    {
        if(isManuallyToggling) return; // Prevent function from being triggered by value change

        isManuallyToggling = true;
        debugMenuToggle.isOn = debugToggle.isOn; // Update UI without triggering function
        isManuallyToggling = false;

        if (debugToggle.isOn)
        {
            Debug.Log("Debug Mode Enabled");
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            textCoroutine = StartCoroutine(VisualText("Debug Mode Enabled"));
            BackButton();
            GameManager.instance.SetDebugMode(true);
        }
        else
        {
            debugMenuToggle.isOn = false;
            Debug.Log("Debug Mode Disabled");
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            textCoroutine = StartCoroutine(VisualText("Debug Mode Disabled"));
            BackButton();
            GameManager.instance.SetDebugMode(false);
        }
    }

    public void ToggleMenuDebugMode()
    {
        if (isManuallyToggling) return; // Prevent execution when toggling programmatically

        isManuallyToggling = true;
        debugToggle.isOn = debugMenuToggle.isOn; // Update the other toggle
        isManuallyToggling = false;

        if (debugMenuToggle.isOn)
        {
            Debug.Log("Debug Mode Enabled");
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            textCoroutine = StartCoroutine(VisualText("Debug Mode Enabled"));
            GameManager.instance.SetDebugMode(true);

        }
        else
        {
            Debug.Log("Debug Mode Disabled");
            debugToggle.isOn = false;
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            textCoroutine = StartCoroutine(VisualText("Debug Mode Disabled"));
            GameManager.instance.SetDebugMode(false);
        }
    }

    private IEnumerator VisualText(string text)
    {
        debugText.enabled = true;
        debugText.text = text;

        yield return new WaitForSeconds(2f);
        debugText.enabled = false;
    }
}
