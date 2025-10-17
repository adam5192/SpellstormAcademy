using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;  // easy tweak per enemy type
    int currentHealth;
    bool frozen = false;
    float freezeTimer = 0f;

    public GameObject fireRunePrefab;
    public GameObject iceRunePrefab;
    public GameObject lightningRunePrefab;

    EnemyController controller; // ref to movement/attack script

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<EnemyController>();
    }

    void Update()
    {
        // thaw timer
        if (frozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0) Unfreeze();
        }
    }

    public void TakeDamage(int dmg)
    {
        // reduce hp, die if 0
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    public void Freeze(float seconds)
    {
        // stop enemy for a bit
        frozen = true;
        freezeTimer = seconds;

        if (controller != null)
            controller.enabled = false; // stop chasing/attacking
    }

    void Unfreeze()
    {
        frozen = false;
        if (controller != null)
            controller.enabled = true;
    }

    void Die()
    {
        // pick one rune at random
        GameObject[] runes = { fireRunePrefab, iceRunePrefab, lightningRunePrefab };
        int randomIndex = Random.Range(0, runes.Length);

        if (runes[randomIndex] != null)
            Instantiate(runes[randomIndex], transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

