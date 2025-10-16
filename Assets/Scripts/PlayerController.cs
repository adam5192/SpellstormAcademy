using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("projectiles + upgrades")]
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;  // time between shots
    private float fireTimer = 0f;

    private int fireRuneCount = 0;
    private int projectileLevel = 1;

    [Header("movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("health")]
    public float maxHealth = 10f;
    private float currentHealth;
    private bool isDead = false;
    private UIManager uiManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // grab ui manager for updates
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
            uiManager.UpdateHealth(currentHealth);
    }

    void Update()
    {
        // movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // shooting timer
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = fireRate;
        }
    }

    void FixedUpdate()
    {
        // smooth physics movement
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void FireProjectile()
    {
        Vector2 shootDir = Vector2.up;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();

        // apply upgrade bonuses dynamically
        p.speed += (projectileLevel - 1) * 2f;
        p.damage += (projectileLevel - 1) * 1f;

        p.Initialize(shootDir);
    }


    public void CollectRune(string runeType)
    {
        if (runeType == "Fire")
        {
            fireRuneCount++;
            uiManager.UpdateRuneUI(fireRuneCount);
            Debug.Log("collected fire rune (" + fireRuneCount + "/3)");

            if (fireRuneCount >= 3)
            {
                UpgradeProjectile();
                fireRuneCount = 0;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"player took {dmg}, hp now {currentHealth}");

        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth);
            uiManager.TriggerDamageFlash(); // flash when hurt
        }

        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        isDead = true;
        Debug.Log("player dead lol");
        GameManager.instance.GameOver();
    }

    void UpgradeProjectile()
    {
        projectileLevel++;
        Debug.Log("projectile upgraded lvl " + projectileLevel);
    }
}
