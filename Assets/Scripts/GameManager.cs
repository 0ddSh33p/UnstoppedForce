using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Title, Playing, Paused, Settings }
    [SerializeField] private GameState currentState = GameState.Title;

    [Header("Scene References")]
    [SerializeField] private GameObject gameplayRoot;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private GameState stateBeforeSettings = GameState.Title;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(this); // keep if you later split into multiple scenes
    }

    private void Start()
    {
        GoToTitle(); // ensure we boot into title
    }

    private void Update()
    {
        // Escape handling
        if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
        else if (currentState == GameState.Settings && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    // ---- State Transitions ----

    public void GoToTitle()
    {
        currentState = GameState.Title;
        Time.timeScale = 1f;                 // normalize timescale
        AudioListener.pause = false;
        gameplayRoot.SetActive(false);
        titlePanel.SetActive(true);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        SetCursor(true);
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        gameplayRoot.SetActive(true);
        titlePanel.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetCursor(false);
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Paused;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;                 // pause time & physics
        AudioListener.pause = true;          // pause audio globally
        SetCursor(true);
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        currentState = GameState.Playing;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetCursor(false);
    }

    public void OpenSettings()
    {
        stateBeforeSettings = currentState;
        currentState = GameState.Settings;
        settingsPanel.SetActive(true);

        // If opened from Title, everything is already stopped.
        // If opened from Pause while time is 0, leave it 0.
        // If you allow opening from Playing, consider setting Time.timeScale = 0 here.
        SetCursor(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        currentState = stateBeforeSettings;

        switch (currentState)
        {
            case GameState.Title:
                titlePanel.SetActive(true);
                pausePanel.SetActive(false);
                SetCursor(true);
                break;
            case GameState.Paused:
                pausePanel.SetActive(true);
                SetCursor(true);
                break;
            case GameState.Playing:
                // If you allowed opening from Playing, resume here:
                Time.timeScale = 1f;
                AudioListener.pause = false;
                SetCursor(false);
                break;
        }
    }

    public void QuitToTitle()
    {
        // Use this from Pause menu
        GoToTitle();
    }

    public void QuitGame()
    {
        // Works in build. In editor this does nothing visible.
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
