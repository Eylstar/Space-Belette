using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneType)
    {
        switch (sceneType)
        {
            case "Start":
                SceneManager.LoadScene("Start");
                break;
            case "Grid":
                SceneManager.LoadScene("Grid");
                break;
            case "Game":
                SceneManager.LoadScene("Game");
                break;
            default:
                Debug.LogError("Invalid scene type");
                break;
        }
    }
}
