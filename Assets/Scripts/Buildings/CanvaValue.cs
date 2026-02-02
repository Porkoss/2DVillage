using UnityEngine;


public class CanvaValue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int value=1;
    public ResourceType resourceType;
    public int maxValue;


    private void Awake()
    {
        //initiliaze (might need to change in the future)
        maxValue = value;
    }


    public void Reset()
    {
        value = maxValue;
    }
}
