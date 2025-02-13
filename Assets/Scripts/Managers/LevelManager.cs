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

    [Header("Event System")]
    [SerializeField] private EventSystem eventSystem;

    // Callback function to be invoked adter fade animation completes
    private Action fadeCallback;
    public static Action startTimer;

    public void Start()
    {
        fadeAnimator = GetComponent<Animator>();
        fadeAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
  

      public void LoadScene(string sceneName)
    {

        if(SceneManager.GetActiveScene().name == "MainMenu" && sceneName == "MainMenu")
        {
            Actions.OnForceStateChange("MainMenu");
            return;
        }

        InputManager.instance.TurnOffInteraction();
        Actions.OnFirstSelect(null);


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
            fakeProgress += Time.unscaledDeltaTime * fakeProgressSpeed;

            // Ensure fake progress doesn't exceed the actual loading progress
            float actualProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            fakeProgress = Mathf.Min(fakeProgress, actualProgress);

            // Update the loading bar
            loadingBar.value = fakeProgress;

            // Wait for the next frame
            yield return null;
        }


        if(sceneName == "MainMenu" || sceneName == "LevelSelect")
        {
            yield return new WaitForSecondsRealtime(0.5f);
            loadingScreen.enabled = false;
            loadOperation.allowSceneActivation = true;
        }


        loadingText.text = "Press Any Key To Continue";
        yield return WaitForAnyKeyPress();

        loadingScreen.enabled = false;
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
                yield break; // Exit the loop and the coroutine
            }

            // Check for any button press on the gamepad
            if (gamepad != null)
            {
                if (gamepad.allControls.OfType<ButtonControl>().Any(button => button.wasPressedThisFrame))
                {
                    yield break; // Exit the loop and the coroutine
                }
            }

            yield return null; // Wait for the next frame
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name.StartsWith("Day"))
        {
            startTimer?.Invoke();
            InputManager.instance.TurnOnInteraction();
        }
        Fade("FadeIn");
    }



    public void Fade(string fadeDir, Action callback = null)
    {
        fadeCallback = callback;
        fadeAnimator.SetTrigger(fadeDir);
    }

    public void FadeAnimationComplete()
    {
        fadeCallback?.Invoke();
        fadeCallback = null;
    }



    public void OnFadeInComplete()
    {
        //Scene currentScene = SceneManager.GetActiveScene();

        //if (currentScene.name.StartsWith("Day"))
        //{
        //    startTimer?.Invoke();
        //    InputManager.instance.TurnOnInteraction();
        //}
    }
}