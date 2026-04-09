using UnityEngine;
using TMPro;
public class UiHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI woodText;
    [SerializeField] TextMeshProUGUI stoneText;
    [SerializeField] TextMeshProUGUI villagerText;

    [SerializeField] TextMeshProUGUI waveCounterText;
    [SerializeField] TextMeshProUGUI timeUntilNextWaveText;
    // Update is called once per frame
    void Update()
    {
        goldText.SetText(": " + StaticClass.Instance.inventory.gold);
        woodText.SetText(": " + StaticClass.Instance.inventory.wood);
        stoneText.SetText(": " + StaticClass.Instance.inventory.stone);
        villagerText.SetText(": " + StaticClass.Instance.inventory.villager);

        waveCounterText.SetText("Wave : "+StaticClass.Instance.GameHandler.GetWaveNumber().ToString());
        timeUntilNextWaveText.SetText("Time until newt wave :"+StaticClass.Instance.GameHandler.GetTimer().ToString());
    }
}
