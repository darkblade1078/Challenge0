using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Episode1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}