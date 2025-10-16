using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f; // seconds between shots
    private float fireTimer = 0f;

    // projectiles and runes
    private int fireRuneCount = 0;
    private int projectileLevel = 1;

    private UIManager uiManager;
    private float maxHealth = 10f;
    private float currentHealth;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
            uiManager.UpdateHealth(currentHealth);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        // handle auto-fire timer
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = fireRate;
        }
    }

    void FireProjectile()
    {
        // Temporary: shoot upward
        Vector2 shootDir = Vector2.up;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(shootDir);
    }

    public void CollectRune(string runeType)
    {
        if (runeType == "Fire")
        {
            fireRuneCount++;
            uiManager.UpdateRuneUI(fireRuneCount);
            Debug.Log("Collected Fire Rune! (" + fireRuneCount + "/3)");

            if (fireRuneCount >= 3)
            {
                UpgradeProjectile();
                fireRuneCount = 0;
            }
        }
    }

    void UpgradeProjectile()
    {
        projectileLevel++;
        Debug.Log("Projectile Upgraded! Level: " + projectileLevel);

        // eg. increase projectile speed and damage
        Projectile proj = projectilePrefab.GetComponent<Projectile>();
        proj.speed += 2f;
        proj.damage += 1;
    }


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
