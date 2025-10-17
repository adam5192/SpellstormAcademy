using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("stats")]
    public int maxHealth = 10;
    int currentHealth;
    bool frozen = false;
    float freezeTimer = 0f;

    [Header("rune drops")]
    public GameObject fireRunePrefab;
    public GameObject iceRunePrefab;
    public GameObject lightningRunePrefab;

    [Header("visual feedback")]
    SpriteRenderer sr;
    Color originalColor;
    bool isFlashing = false;

    EnemyController controller;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<EnemyController>();

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        if (frozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
                Unfreeze();
        }
    }

    // take damage (with optional element)
    public void TakeDamage(int dmg, string element = "Normal")
    {
        currentHealth -= dmg;

        // choose flash color by element
        Color flashColor = Color.red;
        switch (element)
        {
            case "Ice": flashColor = Color.cyan; break;
            case "Lightning": flashColor = Color.yellow; break;
            case "Fire": flashColor = Color.red; break;
            default: flashColor = Color.white; break;
        }

        // start flash
        if (!isFlashing && sr != null)
            StartCoroutine(HitFlash(flashColor, 0.1f));

        if (currentHealth <= 0)
            Die();
    }

    // freeze logic
    public void Freeze(float seconds)
    {
        frozen = true;
        freezeTimer = seconds;

        if (sr != null)
            sr.color = Color.cyan; // show icy tint

        if (controller != null)
            controller.enabled = false;
    }

    void Unfreeze()
    {
        frozen = false;
        if (sr != null)
            sr.color = originalColor;

        if (controller != null)
            controller.enabled = true;
    }

    // flash coroutine
    IEnumerator HitFlash(Color flashColor, float duration)
    {
        isFlashing = true;
        sr.color = flashColor;

        // optional: small scale pop for extra feedback
        transform.localScale *= 1.1f;

        yield return new WaitForSeconds(duration);

        transform.localScale /= 1.1f;
        sr.color = originalColor;
        isFlashing = false;
    }

    // death + rune drop
    void Die()
    {
        GameObject[] runes = { fireRunePrefab, iceRunePrefab, lightningRunePrefab };
        int randomIndex = Random.Range(0, runes.Length);

        if (runes[randomIndex] != null)
            Instantiate(runes[randomIndex], transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
