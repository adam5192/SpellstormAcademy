using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadius = 8f;   // avg spawn distance from player

    [Header("enemy count controls")]
    public int minEnemies = 5;       // floor for how many should exist
    public int maxEnemies = 30;      // hard cap to avoid lag

    [Header("difficulty")]
    public float rampInterval = 15f;   // how often difficulty scales
    public int increasePerRamp = 4;    // bump min/max each ramp
    public int waveSize = 3;           // enemies per wave start
    public int maxWaveSize = 25;       // wave cap so it doesnt explode
    public float waveGrowthRate = 1.3f; // exponential growth factor

    [Header("spawn feel")]
    public float spawnDelay = 0.05f;   // time between spawns in a wave
    public float sectorAngleRange = 60f; // width of the spawn sector

    float rampTimer;

    void Update()
    {
        rampTimer += Time.deltaTime;

        // scale up difficulty every interval
        if (rampTimer >= rampInterval)
        {
            rampTimer = 0f;

            minEnemies += Mathf.CeilToInt(increasePerRamp * 0.5f);
            maxEnemies += increasePerRamp;
            waveSize = Mathf.Min(Mathf.CeilToInt(waveSize * waveGrowthRate), maxWaveSize);
        }

        int activeEnemies = CountActiveEnemies();

        // if under the min, spawn a wave to refill
        if (activeEnemies < minEnemies)
        {
            int toSpawn = Mathf.Min(waveSize, maxEnemies - activeEnemies);
            StartCoroutine(SpawnWave(toSpawn));
        }
    }

    IEnumerator SpawnWave(int amount)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) yield break;

        // pick a random direction for this whole wave
        float waveAngle = Random.Range(0f, 360f);

        for (int i = 0; i < amount; i++)
        {
            // random offset inside a cone around the waveAngle
            float angleOffset = Random.Range(-sectorAngleRange / 2f, sectorAngleRange / 2f);
            float angle = waveAngle + angleOffset;

            // randomize how far from player 
            float radius = Random.Range(spawnRadius * 0.8f, spawnRadius * 1.3f);

            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 spawnPos = (Vector2)player.transform.position + dir * radius;

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // small delay so they don't all pop in at once
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    int CountActiveEnemies()
    {
        // lightweight way to count living enemies
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
