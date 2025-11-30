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
    public float fireRate = 0.25f;
    public float iceRate = 0.8f;
    public float lightningRate = 2f;

    private float fireTimer = 0f;
    private float iceTimer = 0f;
    private float lightningTimer = 0f;

    [Header("runes + xp")]
    public int fireRunes;
    public int iceRunes;
    public int lightningRunes;
    public int xp;
    private PlayerXP xpSystem;

    [Header("player health")]
    public float maxHealth = 10f;
    private float currentHealth;
    private bool isDead = false;
    private UIManager ui;

    // upgrades + leveling
    private UpgradeManager upgradeManager;
    private PlayerXP playerXP;

    void Awake()
    {
        // basic setup
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();
        xpSystem = GetComponent<PlayerXP>(); // grab xp script

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
        // classic top-down movement
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void HandleAutoFire()
    {
        fireTimer -= Time.deltaTime;
        iceTimer -= Time.deltaTime;
        lightningTimer -= Time.deltaTime;

        // aim once per frame at nearest enemy
        Vector2 shootDir = GetAimDirection();

        // fire (basic)
        if (fireTimer <= 0f)
        {
            Shoot(fireProjectile, shootDir);

            float cd = fireRate;
            if (upgradeManager != null)
                cd = fireRate / Mathf.Max(0.01f, upgradeManager.fireRateMultiplier); // faster with upgrades

            fireTimer = cd;
        }

        // ice
        if (iceTimer <= 0f)
        {
            Shoot(iceProjectile, shootDir);

            float cd = iceRate;
            if (upgradeManager != null)
                cd = iceRate / Mathf.Max(0.01f, upgradeManager.iceRateMultiplier);

            iceTimer = cd;
        }

        // lightning
        if (lightningTimer <= 0f)
        {
            Shoot(lightningProjectile, shootDir);

            float cd = lightningRate;
            if (upgradeManager != null)
                cd = lightningRate / Mathf.Max(0.01f, upgradeManager.lightningRateMultiplier);

            lightningTimer = cd;
        }
    }

    // spawns a projectile toward given direction
    void Shoot(GameObject prefab, Vector2 dir)
    {
        if (prefab == null) return;

        GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);

        // let projectile base handle movement + rotation
        ProjectileBase pb = proj.GetComponent<ProjectileBase>();
        if (pb != null)
            pb.SetDirection(dir);
    }

    // get direction toward nearest enemy, or up if none
    Vector2 GetAimDirection()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null)
            return Vector2.up;

        Vector2 playerPos = transform.position;
        Vector2 enemyPos = nearestEnemy.transform.position;
        Vector2 dir = (enemyPos - playerPos);

        if (dir.sqrMagnitude < 0.001f)
            return Vector2.up;

        return dir.normalized;
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

    // specials now aim at mouse position
    void HandleSpecials()
    {
        Vector2 mouseDir = GetMouseDirection();

        // left click = fire special
        if (Input.GetMouseButtonDown(0) && fireRunes >= 5)
        {
            GameObject special = Instantiate(fireSpecial, transform.position, Quaternion.identity);
            special.GetComponent<FireSpecial>()?.SetDirection(mouseDir);
            fireRunes -= 5;
            UpdateRuneUI();
        }

        // right click = ice special
        if (Input.GetMouseButtonDown(1) && iceRunes >= 5)
        {
            GameObject special = Instantiate(iceSpecial, transform.position, Quaternion.identity);
            special.GetComponent<IceSpecial>()?.SetDirection(mouseDir);
            iceRunes -= 5;
            UpdateRuneUI();
        }

        // middle click = lightning special (aoe)
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
            GameObject special = Instantiate(comboSpecial, transform.position, Quaternion.identity);
            special.GetComponent<ComboSpecial>()?.SetDirection(mouseDir);
            fireRunes -= 5;
            iceRunes -= 5;
            lightningRunes -= 5;
            UpdateRuneUI();
        }
    }

    // direction from player to mouse in world space
    Vector2 GetMouseDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - transform.position);
        if (dir.sqrMagnitude < 0.001f)
            return Vector2.up;

        return dir.normalized;
    }

    // rune pickup
    public void AddRune(string type)
    {
        // each rune = 1 xp
        if (xpSystem != null)
            xpSystem.AddXP(1);

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

    // heal from upgrades
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (ui != null)
            ui.UpdateHealth(currentHealth);
    }

    // damage + death
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

        // small shake + pause when player is hit
        if (CameraShake.instance != null)
            CameraShake.instance.Shake(0.15f, 0.25f);

        if (HitPause.instance != null)
            HitPause.instance.DoHitPause(0.05f);

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
