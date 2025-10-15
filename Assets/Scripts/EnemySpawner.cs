using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f; // seconds between spawns
    public float spawnRadius = 6f;

    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnRate;
        }
    }

    void SpawnEnemy()
    {
        // random angle around the player
        float angle = Random.Range(0f, Mathf.PI * 2);
        Vector2 spawnPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // convert to world position relative to player
        Vector2 worldPos = (Vector2)FindObjectOfType<PlayerController>().transform.position + spawnPos;
        Instantiate(enemyPrefab, worldPos, Quaternion.identity);
    }
}

