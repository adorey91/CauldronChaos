using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    [SerializeField] private string debugPassword = "Studio_CLAMS";
    
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Toggle debugToggle;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TextMeshProUGUI debugTitle;

    private Coroutine textCoroutine;

    private void Start()
    {
        passwordInput.onEndEdit.AddListener(delegate { CheckPassword(); });
        backButton.onClick.AddListener(BackButton);

        debugToggle.isOn = false;
        debugToggle.gameObject.SetActive(false);
        debugText.enabled = false;
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
        if (debugToggle.isOn)
        {
            Debug.Log("Debug Mode Enabled");
            GameManager.instance.SetDebugMode(true);
        }
        else
        {
            Debug.Log("Debug Mode Disabled");
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
