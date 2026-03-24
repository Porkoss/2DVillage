using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        Debug.Log("Starting Game");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TestScene");////TODO UPDATE THIS
        
    }
}
