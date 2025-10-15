using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private VisualElement pauseMenuContainer;
    private Button buttonResume;
    private Button buttonRetry;
    private Button buttonSettings;
    private Button buttonQuit;

    private bool isPaused = false;

    void Start()
    {
        // Get the root VisualElement from the UIDocument
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Grab the pause menu container
        pauseMenuContainer = root.Q<VisualElement>("pauseMenuContainer");

        // Grab the buttons by name
        buttonResume = root.Q<Button>("buttonResume");
        buttonRetry = root.Q<Button>("buttonRetry");
        buttonSettings = root.Q<Button>("buttonSettings");
        buttonQuit = root.Q<Button>("buttonQuit");

        // Hook up button events
        buttonResume.clicked += OnResumeClicked;
        buttonRetry.clicked += OnRetryClicked;
        buttonSettings.clicked += OnSettingsClicked;
        buttonQuit.clicked += OnQuitClicked;

        // Hide menu by default
        pauseMenuContainer.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
            pauseMenuContainer.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1f; // Resume
            pauseMenuContainer.style.display = DisplayStyle.None;
        }
    }

    void OnResumeClicked()
    {
        TogglePauseMenu();
    }

    void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSettingsClicked()
    {
        Debug.Log("Settings menu would open here.");
        // You could show another UXML overlay or sub-menu.
    }

    void OnQuitClicked()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
