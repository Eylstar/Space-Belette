using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader 
{
    public static void LoadScene(string sceneType)
    {
        switch (sceneType)
        {
            case "Start":
                SceneManager.LoadScene("StartScene");
                break;
            case "Intro":
                SceneManager.LoadScene("Intro");
                break;            
            case "Grid":
                SceneManager.LoadScene("Grid");
                break;
            case "CharacterSelection":
                SceneManager.LoadScene("CharacterSelection");
                break;
            case "MissionSelection":
                SceneManager.LoadScene("MissionSelection");
                break;
            case "Space":
                SceneManager.LoadScene("Space");
                break;
            case "Exit":
                Application.Quit();
                break;
            default:
                Debug.LogError("Invalid scene type");
                break;
        }
    }
}
