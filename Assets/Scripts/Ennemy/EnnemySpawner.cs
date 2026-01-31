using UnityEngine;

public class EnnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject ennemyPrefab;
    [SerializeField] GameObject spawnPoint;

    public int  spawnNumber = 1;
    bool spawning = false;
    [SerializeField] float delay = 0.5f;
    float currentIteration = 0f;
    float runningThreshold = 0f;
    public void SpawnEnnemy()
    {
        spawning = true;
        currentIteration = 0f;
        runningThreshold = 0f;
    }

    private void Update()
    {
        if (spawning)
        {
            if (currentIteration >= spawnNumber * delay)
            {
                spawning = false;
            }
            else if(currentIteration >=runningThreshold * delay)
            {
                runningThreshold++;
                SpawnOneEnnemy();
            }

            currentIteration += Time.deltaTime;
        }
    }

    private void SpawnOneEnnemy()
    {
        Instantiate(ennemyPrefab, spawnPoint.transform.position, Quaternion.identity);
    }
}
