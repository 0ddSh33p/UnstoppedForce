using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private VisualElement pauseMenuContainer;
    private Button buttonResume;
    private Button buttonRetry;
    private Button buttonMenu; // Renamed to match UXML name "buttonMenu"
    private Button buttonSettings;
    private Button buttonQuit;

    private bool isPaused = false;

    // INSTRUCTION: Type the exact name of your Main Menu scene in the Unity Inspector
    [SerializeField] private string mainMenuSceneName = "MainMenu"; 

    void Start()
    {
        // 1. Get the root VisualElement
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("PauseMenu: Missing UIDocument component!");
            return;
        }

        var root = uiDocument.rootVisualElement;

        // 2. Query the elements by name (Must match UXML names exactly)
        pauseMenuContainer = root.Q<VisualElement>("pauseMenuContainer");
        buttonResume = root.Q<Button>("buttonResume");
        buttonRetry = root.Q<Button>("buttonRetry");
        buttonMenu = root.Q<Button>("buttonMenu"); // Looking for the new button
        buttonSettings = root.Q<Button>("buttonSettings");
        buttonQuit = root.Q<Button>("buttonQuit");

        // 3. Subscribe to Click Events
        if (buttonResume != null) buttonResume.clicked += OnResumeClicked;
        
        if (buttonRetry != null) buttonRetry.clicked += OnRetryClicked;
        
        if (buttonMenu != null) buttonMenu.clicked += OnMenuClicked;
        else Debug.LogWarning("PauseMenu: Could not find button named 'buttonMenu' in UXML.");

        if (buttonSettings != null) buttonSettings.clicked += OnSettingsClicked;
        
        if (buttonQuit != null) buttonQuit.clicked += OnQuitClicked;

        // 4. Ensure Menu is hidden at start
        if (pauseMenuContainer != null)
        {
            pauseMenuContainer.style.display = DisplayStyle.None;
        }
    }

    void Update()
    {
        // Toggle pause when pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuContainer == null) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Freezes the game time
            pauseMenuContainer.style.display = DisplayStyle.Flex; // Shows the menu
        }
        else
        {
            Time.timeScale = 1f; // Unfreezes the game time
            pauseMenuContainer.style.display = DisplayStyle.None; // Hides the menu
        }
    }

    // --- Button Functions ---

    void OnResumeClicked()
    {
        TogglePauseMenu();
    }

    void OnRetryClicked()
    {
        // Unfreeze time before reloading, or the game will be stuck paused
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnMenuClicked()
    {
        // Unfreeze time before leaving
        Time.timeScale = 1f;
        
        // Load the scene defined in the Inspector variable
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnSettingsClicked()
    {
        Debug.Log("Settings menu clicked. (Add settings logic here)");
    }

    void OnQuitClicked()
    {
        Time.timeScale = 1f;
        Debug.Log("Quitting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}