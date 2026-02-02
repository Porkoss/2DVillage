using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject pausePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MainMenu()
    {
        
        SceneManager.LoadScene("Title");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
