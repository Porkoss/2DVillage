
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    int waveCounter= 0;

    [SerializeField]float currentTimer = 60f;
    [SerializeField]float waveCoolDown = 60f;
    List<EnnemySpawner> ennemySpawners;

    private void Start()
    {
        ennemySpawners = new List<EnnemySpawner>();
        var temp = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject gameObject in temp)
        {
            ennemySpawners.Add(gameObject.GetComponent<EnnemySpawner>());
        }
        
    }

    private void Update()
    {
        if(currentTimer<= 0)
        {
            currentTimer =waveCoolDown;
            LaunchIterativeWaves();
        }
        currentTimer -= Time.deltaTime;
    }

    void LaunchIterativeWaves()
    {
        waveCounter++;
        int spawnerCount = ennemySpawners.Count;
        for(int i = 0;i < spawnerCount; i++)
        {
            ennemySpawners[i].spawnNumber = waveCounter;
            ennemySpawners[i].SpawnEnnemy();

            
        }
        foreach (var building in StaticClass.Instance.listOfBuiltBuilding)
        {
            building.GetComponent<Building>().RestoreArmy();
        }

    }

    public float  GetTimer()
    {
        return Mathf.Round(currentTimer);
    }

    public int  GetWaveNumber()
    {
        return waveCounter;
    }

    

}
