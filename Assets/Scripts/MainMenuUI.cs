using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI: MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("level_1.1_junkyard");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    [Header("UI Panels")]
    // Reference to your Settings Panel object in the hierarchy
    public GameObject settingsPanel;
    
    // Optional: Reference to the Main Menu container if you want to hide it
    // when Settings are open (prevents clicking buttons behind the settings)
    public GameObject mainMenuPanel;

    void Start()
    {
        // Ensure Settings are closed when the game starts
        settingsPanel.SetActive(false);
        
        // Ensure Main Menu is open
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // Call this function on the "Options/Settings" Button
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        
        // Optional: Hide the main menu so it's not distracting
        if(mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }
// Call this function on the "Back" Button inside the Settings Panel
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        
        // Optional: Bring the main menu back
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
   
}
