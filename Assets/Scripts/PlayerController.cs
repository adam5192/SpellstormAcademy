using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("base projectile stuff")]
    public GameObject fireProjectile;
    public GameObject iceProjectile;
    public GameObject lightningProjectile;

    [Header("special attacks")]
    public GameObject fireSpecial;
    public GameObject iceSpecial;
    public GameObject lightningSpecial;
    public GameObject comboSpecial;

    [Header("movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("fire rates (seconds per shot)")]
    public float fireRate = 0.5f;
    public float iceRate = 1f;
    public float lightningRate = 3f;

    private float fireTimer = 0f;
    private float iceTimer = 0f;
    private float lightningTimer = 0f;

    [Header("runes + xp")]
    public int fireRunes;
    public int iceRunes;
    public int lightningRunes;
    public int xp;

    [Header("player health")]
    public float maxHealth = 10f;
    private float currentHealth;
    private bool isDead = false;
    private UIManager ui;

    void Awake()
    {
        // basic setup
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();

        if (ui != null)
            ui.UpdateHealth(currentHealth);
    }

    void Update()
    {
        HandleMovementInput();
        HandleAutoFire();
        HandleSpecials();
    }

    void FixedUpdate()
    {
        // classic top-down movement (same as your old code)
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // -----------------------------
    // movement input (wasd/arrows)
    // -----------------------------
    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    // -----------------------------
    // auto-firing logic (like old code)
    // -----------------------------
    void HandleAutoFire()
    {
        fireTimer -= Time.deltaTime;
        iceTimer -= Time.deltaTime;
        lightningTimer -= Time.deltaTime;

        // fire (basic)
        if (fireTimer <= 0f)
        {
            Shoot(fireProjectile);
            fireTimer = fireRate;
        }

        // ice
        if (iceTimer <= 0f)
        {
            Shoot(iceProjectile);
            iceTimer = iceRate;
        }

        // lightning
        if (lightningTimer <= 0f)
        {
            Shoot(lightningProjectile);
            lightningTimer = lightningRate;
        }
    }

    // -----------------------------
    // spawns a projectile toward neares enemy
    // -----------------------------
    void Shoot(GameObject prefab)
    {
        if (prefab == null) return;

        // find the nearest enemy
        GameObject nearestEnemy = FindNearestEnemy();
        Vector2 shootDir = Vector2.up; // default dir (no enemies)

        if (nearestEnemy != null)
        {
            shootDir = (nearestEnemy.transform.position - transform.position).normalized;
        }

        // spawn projectile
        GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);

        // rotate projectile sprite to face target (optional visual)
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg - 90f;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);

        // apply movement
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj != null)
            rbProj.velocity = shootDir * 10f; // tweak speed if needed
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        Vector2 playerPos = transform.position;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(playerPos, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }

        return nearest;
    }


    // -----------------------------
    // handle button press specials
    // -----------------------------
    void HandleSpecials()
    {
        // left click = fire special
        if (Input.GetMouseButtonDown(0) && fireRunes >= 5)
        {
            Instantiate(fireSpecial, transform.position, Quaternion.identity);
            fireRunes -= 5;
            UpdateRuneUI();
        }

        // right click = ice special
        if (Input.GetMouseButtonDown(1) && iceRunes >= 5)
        {
            Instantiate(iceSpecial, transform.position, Quaternion.identity);
            iceRunes -= 5;
            UpdateRuneUI();
        }

        // middle click = lightning special
        if (Input.GetMouseButtonDown(2) && lightningRunes >= 5)
        {
            Instantiate(lightningSpecial, transform.position, Quaternion.identity);
            lightningRunes -= 5;
            UpdateRuneUI();
        }

        // E = combo (all 3 full)
        if (Input.GetKeyDown(KeyCode.E) &&
            fireRunes >= 5 && iceRunes >= 5 && lightningRunes >= 5)
        {
            Instantiate(comboSpecial, transform.position, Quaternion.identity);
            fireRunes -= 5;
            iceRunes -= 5;
            lightningRunes -= 5;
            UpdateRuneUI();
        }
    }

    // -----------------------------
    // rune pickup
    // -----------------------------
    public void AddRune(string type)
    {
        xp++;

        switch (type)
        {
            case "Fire": fireRunes++; break;
            case "Ice": iceRunes++; break;
            case "Lightning": lightningRunes++; break;
        }

        UpdateRuneUI();
    }

    void UpdateRuneUI()
    {
        if (ui != null)
        {
            ui.UpdateRuneUI(fireRunes, iceRunes, lightningRunes);
        }
    }

    // -----------------------------
    // damage + death
    // -----------------------------
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (ui != null)
        {
            ui.UpdateHealth(currentHealth);
            ui.TriggerDamageFlash();
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        if (ui != null)
            ui.ShowGameOverPanel();

        Debug.Log("player died");
        enabled = false;
    }
}
