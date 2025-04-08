using UnityEngine;

public class SpawnerColetavel : MonoBehaviour
{
    public GameObject collectablePrefab;
    public float spawnInterval = 2f;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    private void Start()
    {
        // Start spawning collectables at regular intervals
        InvokeRepeating("SpawnAtRandomPosition", 0f, spawnInterval);
    }

    private void SpawnAtRandomPosition()
    {
        // Generate a random position within the spawn area
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // Call SpawnCollectable with the generated position
        SpawnCollectable(spawnPosition);
    }

    public void SpawnCollectable(Vector2 spawnPosition)
    {
        Instantiate(collectablePrefab, spawnPosition, Quaternion.identity);
    }

    public void StopSpawning()
    {
        CancelInvoke("SpawnAtRandomPosition");
    }
}