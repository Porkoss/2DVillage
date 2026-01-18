using UnityEngine;
using TMPro;
public class UiHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI woodText;
    [SerializeField] TextMeshProUGUI stoneText;
    // Update is called once per frame
    void Update()
    {
        goldText.SetText(": " + StaticClass.Instance.inventory.gold);
        woodText.SetText(": " + StaticClass.Instance.inventory.wood);
        stoneText.SetText(": " + StaticClass.Instance.inventory.stone);
    }
}
