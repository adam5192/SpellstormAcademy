using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;       // starting seconds between spawns
    public float spawnRadius = 6f;
    public float minSpawnRate = 0.3f;  // how fast it can get at most
    public float difficultyRamp = 0.02f; // how much to increase per second

    private float spawnTimer = 0f;

    void Update()
    {
        // count down until next spawn
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnRate;
        }

        // gradually decrease spawn rate over time
        spawnRate = Mathf.Max(minSpawnRate, spawnRate - (difficultyRamp * Time.deltaTime));
    }

    void SpawnEnemy()
    {
        // pick random angle around player
        float angle = Random.Range(0f, Mathf.PI * 2);
        Vector2 spawnPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // get player position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Vector2 worldPos = (Vector2)player.transform.position + spawnPos;
        Instantiate(enemyPrefab, worldPos, Quaternion.identity);
    }
}
