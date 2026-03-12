using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Loads a scene by its name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quits the application
    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}
