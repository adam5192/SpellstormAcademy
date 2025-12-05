using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("references")]
    public GameObject basicEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tankEnemyPrefab;
    private PlayerController player;
    private Camera mainCam;

    [Header("spawn control")]
    public int minEnemies = 10;         // keep at least this many
    public int maxEnemies = 70;         // hard cap to avoid lag
    public int baseWaveSize = 8;        // enemies per spawn cycle
    public float spawnDelay = 0.02f;    // delay between enemies in a lane
    public float checkInterval = 0.2f;  // how often to check population
    private float checkTimer = 0f;

    [Header("spawn placement")]
    public float edgeBuffer = 1.2f;     // extra distance outside screen
    public float positionNoise = 1.5f;
    public float laneSpacing = 2.2f;

    // how far from player
    [Range(0.4f, 1f)]
    public float screenRadiusFactor = 0.6f;

    // hard minimum world distance from player where enemies can spawn
    public float minSafeDistanceFromPlayer = 7f;

    [Header("arena bounds")]
    public float minX = -24.6f;         
    public float maxX = 24.6f;        
    public float minY = -14.6f;    
    public float maxY = 14.6f;         

    [Header("difficulty scaling")]
    public float warmupDuration = 45f;      // first X seconds are softer

    public float earlyRampInterval = 10f;   // how often to ramp during warmup
    public float lateRampInterval = 6f;     // how often to ramp after warmup

    public float earlyRampMultiplier = 1.10f;
    public float lateRampMultiplier = 1.20f;

    private float rampTimer = 0f;
    private float totalRunTime = 0f;

    [Header("wave variety")]
    public int minDirections = 2;       // how many angles per wave
    public int maxDirections = 4;

    [Header("enemy type chances")]
    [Range(0f, 1f)] public float fastChance = 0.2f;
    [Range(0f, 1f)] public float tankChance = 0.1f;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (player == null || mainCam == null) return;

        totalRunTime += Time.deltaTime;
        rampTimer += Time.deltaTime;
        checkTimer += Time.deltaTime;

        bool inWarmup = totalRunTime < warmupDuration;
        float currentInterval = inWarmup ? earlyRampInterval : lateRampInterval;
        float currentMultiplier = inWarmup ? earlyRampMultiplier : lateRampMultiplier;

        // ramp difficulty over time
        if (rampTimer >= currentInterval)
        {
            rampTimer = 0f;

            // slowly increase enemy counts
            minEnemies = Mathf.Min(Mathf.CeilToInt(minEnemies * currentMultiplier), 40);
            maxEnemies = Mathf.Min(Mathf.CeilToInt(maxEnemies * currentMultiplier), 120);

            // increase wave size a bit
            baseWaveSize = Mathf.Min(Mathf.CeilToInt(baseWaveSize * currentMultiplier), 20);
        }

        if (checkTimer < checkInterval)
            return;
        checkTimer = 0f;

        int activeEnemies = CountActiveEnemies();

        if (activeEnemies < minEnemies)
        {
            int toSpawn = Mathf.Min(baseWaveSize, maxEnemies - activeEnemies);
            SpawnAroundPlayer(toSpawn);
        }
    }

    public int GetActiveEnemies()
    {
        return CountActiveEnemies();
    }

    void SpawnAroundPlayer(int totalEnemies)
    {
        if (totalEnemies <= 0) return;

        int directions = Random.Range(minDirections, maxDirections + 1); // 2–4
        directions = Mathf.Max(1, directions);

        int enemiesPerDir = Mathf.Max(1, totalEnemies / directions);

        for (int i = 0; i < directions; i++)
        {
            float angle = Random.Range(0f, 360f);
            StartCoroutine(SpawnMultiLaneWave(enemiesPerDir, angle, 0f));
        }
    }

    IEnumerator SpawnMultiLaneWave(int totalEnemies, float baseAngle, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        int lanes = Random.Range(1, 3); // 1–2 lanes per direction
        int enemiesPerLane = Mathf.Max(1, totalEnemies / lanes);

        for (int lane = 0; lane < lanes; lane++)
        {
            float laneAngleOffset = (lane - (lanes - 1) / 2f) * 6f;
            float laneSideOffset = (lane - (lanes - 1) / 2f) * laneSpacing;

            for (int i = 0; i < enemiesPerLane; i++)
            {
                Vector2 spawnPos = GetSpawnPositionNearScreenEdge(baseAngle + laneAngleOffset, laneSideOffset);

                GameObject prefab = GetRandomEnemyPrefab();
                if (prefab != null)
                    Instantiate(prefab, spawnPos, Quaternion.identity);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    Vector2 GetSpawnPositionNearScreenEdge(float angle, float laneOffset = 0f)
    {
        Vector2 playerPos2D = player.transform.position;

        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                  Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        // spawn further out around the player, based on screen size
        float spawnDistance = Mathf.Max(camWidth, camHeight) * screenRadiusFactor + edgeBuffer;

        Vector2 spawnBase = playerPos2D + dir * spawnDistance;

        Vector2 perpendicular = new Vector2(-dir.y, dir.x);
        spawnBase += perpendicular * laneOffset;

        // small random offset
        spawnBase += Random.insideUnitCircle * positionNoise;

        // enforce a hard minimum safe distance from the player
        Vector2 toSpawn = spawnBase - playerPos2D;
        float dist = toSpawn.magnitude;
        if (dist < minSafeDistanceFromPlayer)
        {
            spawnBase = playerPos2D + toSpawn.normalized * minSafeDistanceFromPlayer;
        }

        // clamp inside arena bounds so enemies never spawn past walls
        float clampedX = Mathf.Clamp(spawnBase.x, minX + 0.5f, maxX - 0.5f);
        float clampedY = Mathf.Clamp(spawnBase.y, minY + 0.5f, maxY - 0.5f);

        return new Vector2(clampedX, clampedY);
    }

    GameObject GetRandomEnemyPrefab()
    {
        float r = Random.value;

        if (r < fastChance && fastEnemyPrefab != null)
            return fastEnemyPrefab;

        if (r < fastChance + tankChance && tankEnemyPrefab != null)
            return tankEnemyPrefab;

        return basicEnemyPrefab;
    }

    int CountActiveEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null || mainCam == null) return;

        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        float radius = Mathf.Max(camWidth, camHeight) * screenRadiusFactor + edgeBuffer;

        // spawn radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.transform.position, radius);

        // safe no-spawn radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.transform.position, minSafeDistanceFromPlayer);
    }
}
