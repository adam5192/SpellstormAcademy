using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    private PlayerController player;
    private Camera mainCam;

    [Header("Spawn Control")]
    public int minEnemies = 10;        
    public int maxEnemies = 60;         // hard cap to avoid lag
    public int baseWaveSize = 3;        // enemies per ewave
    public float spawnDelay = 0.04f;    // small delay between enemies in a wave
    public float checkInterval = 0.4f;  // how often check for spawn

    [Header("Spawn Placement")]
    public float edgeBuffer = 2f;       // how far outside the screen to spawn
    public float positionNoise = 1.5f;  // add some randomness to avoid perfect lines
    public float laneSpacing = 2.5f;    // distance between multi-lane spawns

    [Header("Difficulty Scaling")]
    public float rampInterval = 10f;    // how often difficulty ramps up (seconds)
    private float rampTimer;
    public float rampMultiplier = 1.15f; // scaling factor per ramp

    [Header("Direction Persistence")]
    public float directionChangeInterval = 7f; // how long waves come from the same side
    private float currentWaveAngle;
    private float directionTimer;

    [Header("Wave Variety")]
    public float multiWaveChance = 0.4f;  // 40% chance to spawn a secondary wave
    public float secondaryWaveDelay = 2f; // delay before secondary wave spawns

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;
        currentWaveAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (player == null || mainCam == null) return;

        rampTimer += Time.deltaTime;
        directionTimer += Time.deltaTime;

        // ramp difficulty a bit faster over time
        if (rampTimer >= rampInterval)
        {
            rampTimer = 0f;

            // scale counts slightly each interval
            minEnemies = Mathf.CeilToInt(minEnemies * rampMultiplier);
            maxEnemies = Mathf.CeilToInt(maxEnemies * rampMultiplier);

            // slightly grow the wave size too, capped so it stays reasonable
            baseWaveSize = Mathf.Min(baseWaveSize + 1, 10);
        }

        // rotate spawn direction every few seconds for variety
        if (directionTimer >= directionChangeInterval)
        {
            directionTimer = 0f;
            currentWaveAngle += Random.Range(70f, 130f); // switch to another sector
        }

        // keep checking population often for steady flow
        int activeEnemies = CountActiveEnemies();

        if (activeEnemies < minEnemies)
        {
            int toSpawn = Mathf.Min(baseWaveSize, maxEnemies - activeEnemies);
            StartCoroutine(SpawnMultiLaneWave(toSpawn, currentWaveAngle));

            // small chance of secondary wave from a different side
            if (Random.value < multiWaveChance)
            {
                float altAngle = currentWaveAngle + Random.Range(120f, 180f);
                StartCoroutine(SpawnMultiLaneWave(toSpawn / 2, altAngle, secondaryWaveDelay));
            }
        }
    }

    IEnumerator SpawnMultiLaneWave(int totalEnemies, float baseAngle, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        int lanes = Random.Range(2, 4); // between 2 and 3 lanes
        int enemiesPerLane = Mathf.Max(1, totalEnemies / lanes);

        for (int lane = 0; lane < lanes; lane++)
        {
            // spread each lane slightly so it's not a straight column
            float laneAngleOffset = (lane - (lanes - 1) / 2f) * 8f;
            float laneSideOffset = (lane - (lanes - 1) / 2f) * laneSpacing;

            for (int i = 0; i < enemiesPerLane; i++)
            {
                Vector2 spawnPos = GetSpawnPositionNearScreenEdge(baseAngle + laneAngleOffset, laneSideOffset);

                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                // small delay between spawns so they feel like they’re walking in
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    Vector2 GetSpawnPositionNearScreenEdge(float angle, float laneOffset = 0f)
    {
        Vector2 playerPos2D = player.transform.position;

        // grab camera bounds in world space
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        // direction for this wave
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        // spawn just outside the visible area
        float spawnDistance = Mathf.Max(camWidth, camHeight) * 0.55f + edgeBuffer;

        // base spawn position slightly past the camera edge
        Vector2 spawnBase = playerPos2D + dir * spawnDistance;

        // offset lanes sideways so it's a band instead of one line
        Vector2 perpendicular = new Vector2(-dir.y, dir.x);
        spawnBase += perpendicular * laneOffset;

        // add noise so enemies arent in a perfect line
        spawnBase += Random.insideUnitCircle * positionNoise;

        return spawnBase;
    }

    int CountActiveEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null || mainCam == null) return;

        Gizmos.color = Color.yellow;
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        float radius = Mathf.Max(camWidth, camHeight) * 0.55f + edgeBuffer;
        Gizmos.DrawWireSphere(player.transform.position, radius);
    }
}
