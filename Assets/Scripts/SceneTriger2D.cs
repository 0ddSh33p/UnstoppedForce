using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger2D : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnPointNameInNextScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene(sceneToLoad);
    }
}
