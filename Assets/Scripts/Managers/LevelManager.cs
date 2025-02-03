using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField] private Canvas loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private float fakeProgressSpeed = 0.2f;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Scene Fade")]
    private Animator fadeAnimator;

    [Header("LevelButtons")]
    [SerializeField] private Button[] levelButtons;

    [Header("Save")]
    [SerializeField] private SaveLoad saveLoad;

    [Header("Event System")]
    [SerializeField] private EventSystem eventSystem;

    // Callback function to be invoked adter fade animation completes
    private Action fadeCallback;
    public static Action startTimer;


    public void Start()
    {
        fadeAnimator = GetComponent<Animator>();
        fadeAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (i < saveLoad.CheckUnlockedDays())
            {
                levelButtons[i].interactable = true;
                if (saveLoad.CheckScore(i) == 0) return;

                buttonText.text = $"Day {i + 1}\nScore: {saveLoad.CheckScore(i)}";
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        eventSystem.SetSelectedGameObject(null);
        Debug.Log($"LoadScene called: {sceneName}");

        Fade("FadeOut", () =>
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            switch (sceneName)
            {
                case "MainMenu": Actions.OnForceStateChange("MainMenu"); break;
                case "Intro": Actions.OnForceStateChange("Intro"); break;
                case "LevelSelect":
                    UpdateButtons();
                    Actions.OnForceStateChange("LevelSelect"); 
                    break;
                case string name when name.StartsWith("Day"): Actions.OnForceStateChange("Gameplay"); break;
                case "GameOver": Actions.OnForceStateChange("GameOver"); break;
            }

            loadingScreen.enabled = true;
            loadingText.text = "Loading...";

            StartCoroutine(LoadAsync(sceneName));
        });
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        // Load the scene asynchronously
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // Prevent the scene from activating immediately, even if it's fully loaded
        loadOperation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 1f)
        {
            // Gradually increase the fake progress
            fakeProgress += Time.deltaTime * fakeProgressSpeed;

            // Ensure fake progress doesn't exceed the actual loading progress
            float actualProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            fakeProgress = Mathf.Min(fakeProgress, actualProgress);

            // Update the loading bar
            loadingBar.value = fakeProgress;

            // Wait for the next frame
            yield return null;
        }

        if(sceneName != "LevelSelect" && sceneName != "MainMenu")
        {
            loadingText.text = "Press Any Key To Continue";

            // Wait for any key press
            yield return WaitForAnyKeyPress();
        }

        loadingScreen.enabled = false;
        // Activate the scene after the player presses the button
        loadOperation.allowSceneActivation = true;
    }

    private IEnumerator WaitForAnyKeyPress()
    {
        // Cache the keyboard and gamepad references
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        while (true)
        {
            // Check for any key press on the keyboard
            if (keyboard?.anyKey.wasPressedThisFrame == true)
            {
                Debug.Log("A key was pressed.");
                yield break; // Exit the loop and the coroutine
            }

            // Check for any button press on the gamepad
            if (gamepad != null)
            {
                if (gamepad.allControls.OfType<ButtonControl>().Any(button => button.wasPressedThisFrame))
                {
                    Debug.Log("A button was pressed.");
                    yield break; // Exit the loop and the coroutine
                }
            }

            yield return null; // Wait for the next frame
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Fade in and delay the day start countdown until after fading is complete
        Fade("FadeIn");
    }


    public void Fade(string fadeDir, Action callback = null)
    {
        Debug.Log($"Fade triggered: {fadeDir}");
        fadeCallback = callback;
        fadeAnimator.SetTrigger(fadeDir);
    }

    public void FadeAnimationComplete()
    {
        Debug.Log("Fade Animation Complete");
        // Ensure the callback is executed only after fade-in is complete
        fadeCallback?.Invoke();
        fadeCallback = null; // Clear the callback to prevent accidental reuse
    }

    
    public void OnFadeInComplete()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name.StartsWith("Day"))
        {
            startTimer?.Invoke();
        }
    }
}