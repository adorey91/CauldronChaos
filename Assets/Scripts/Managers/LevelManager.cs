using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField] private Canvas loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private float fakeProgressSpeed = 0.2f;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Scene Fade")]
    public Animator fadeAnimator;

    // Callback function to be invoked adter fade animation completes
    private System.Action fadeCallback;


    public void Start()
    {
        fadeAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    
    public void LoadScene(string sceneName)
    {
        Fade("FadeOut", () =>
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            switch (sceneName)
            {
                case "MainMenu": Actions.OnForceStateChange("MainMenu"); break;
                case "Intro": Actions.OnForceStateChange("Intro"); break;
                case "LevelSelect": Actions.OnForceStateChange("LevelSelect"); break;
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

        loadingText.text = "Press Any Key To Continue";

        // Wait for any key press
        yield return WaitForAnyKeyPress();

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

        Fade("FadeIn");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Fade(string fadeDir, System.Action callback = null)
    {
        fadeCallback = callback;
        fadeAnimator.SetTrigger(fadeDir);
    }

    public void FadeAnimationComplete()
    {
        // Invoke the callback if it's not null
        fadeCallback?.Invoke();
    }
}