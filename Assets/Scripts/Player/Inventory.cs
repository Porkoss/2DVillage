using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int gold = 0;
    public int wood = 0;
    public int stone = 0;

    public bool Substract(CanvaValue canvaValue)
    {
        switch (canvaValue.resourceType)
        {
            case ResourceType.Gold:
                if (gold >= 1)
                {
                    gold--;
                    return true;
                }
                else
                {
                    return false;
                }
            case ResourceType.Wood:
                if (wood >= 1)
                {
                    wood--;
                    return true;
                }
                else
                {
                    return false;
                }
            case ResourceType.Stone:
                if (stone >= 1)
                {
                    stone--;
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                Debug.Log("Wrong type of Resource type shoud be impossible in enum");
                return false;
        }
    }
}
