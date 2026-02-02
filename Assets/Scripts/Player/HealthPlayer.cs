using UnityEngine;

public class HealthPlayer : Health
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Death()
    {
        
        StaticClass.Instance.GameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}
