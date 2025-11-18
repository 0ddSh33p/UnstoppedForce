using UnityEngine;
using UnityEngine.SceneManagement;

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
}
