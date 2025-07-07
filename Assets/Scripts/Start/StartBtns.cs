using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtns : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Intro");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("ShipBuilder");
    }
}
