using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneIndex = 0;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
