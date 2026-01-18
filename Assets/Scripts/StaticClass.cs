using UnityEngine;

public class StaticClass: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static StaticClass Instance { get; private set; }

    public GameObject player;

    public Inventory inventory;

    public UiHandler uiHandler;

    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
