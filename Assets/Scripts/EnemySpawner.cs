using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public float spawnRateIncrease = 0.1f;
    public float minDistanceFromPlayer = 2f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition;
        int attempts = 0;
        bool validPosition = false;

        do
        {
            spawnPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (Vector2.Distance(spawnPosition, player.position) >= minDistanceFromPlayer)
            {
                validPosition = true;
            }

            attempts++;
        } while (!validPosition && attempts < 10);

        if (validPosition)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }

        spawnInterval = Mathf.Max(0.1f, spawnInterval - spawnRateIncrease);
        CancelInvoke("SpawnEnemy");
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);
    }

    public void StopSpawning()
    {
        CancelInvoke("SpawnEnemy");
    }
}