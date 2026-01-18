using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Unpause()
    {
        Time.timeScale = 1f;
        pausePrefab.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
