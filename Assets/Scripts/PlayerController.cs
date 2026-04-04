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

    [Header("sound effects")]
    public AudioClip fireShootSfx;
    public float fireShootVolume = 1f;
    public AudioClip iceShootSfx;
    public float iceShootVolume = 1f;
    public AudioClip lightningShootSfx;
    public float lightningShootVolume = 1f;
    public AudioClip fireSpecialSfx;
    public float fireSpecialVolume = 1f;
    public AudioClip iceSpecialSfx;
    public float iceSpecialVolume = 1f;
    public AudioClip lightningSpecialSfx;
    public float lightningSpecialVolume = 1f;
    public AudioClip comboSpecialSfx;
    public float comboSpecialVolume = 1f;
    public AudioClip ouchSfx;

    [Header("movement loop")]
    public AudioClip moveLoopSfx;
    public AudioClip empoweredMoveLoopSfx;
    public float moveLoopVolume = 0.3f;

    [Header("pitch randomization")]
    public float minPitch = 0.94f;
    public float maxPitch = 1.06f;

    [Header("special ui")]
    public SpecialUIIcon fireUI;
    public SpecialUIIcon iceUI;
    public SpecialUIIcon lightningUI;
    public SpecialUIIcon comboUI;


    // sfx cooldowns to not spam audio
    private float fireSfxTimer = 0f;
    private float iceSfxTimer = 0f;
    private float lightningSfxTimer = 0f;


    private AudioSource audioSrc;
    private AudioSource moveLoopSource;


    // upgrades + leveling
    private UpgradeManager upgradeManager;
    private PlayerXP playerXP;
    private UpgradeMenu upgradeMenu;

    void Start()
    {
        fireTimer = fireRate;
        iceTimer = iceRate;
        lightningTimer = lightningRate;
    }


    void Awake()
    {
        // basic setup
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();
        xpSystem = GetComponent<PlayerXP>(); // grab xp script
        upgradeMenu = FindObjectOfType<UpgradeMenu>();
        upgradeManager = GetComponent<UpgradeManager>();
        audioSrc = GetComponent<AudioSource>();

        if (audioSrc != null)
        {
            audioSrc.playOnAwake = false;
            audioSrc.loop = false;
            audioSrc.spatialBlend = 0f;
        }

        moveLoopSource = gameObject.AddComponent<AudioSource>();
        moveLoopSource.playOnAwake = false;
        moveLoopSource.loop = true;
        moveLoopSource.spatialBlend = 0f;
        moveLoopSource.volume = moveLoopVolume;

        if (ui != null)
            ui.UpdateHealth(currentHealth);
    }


    void Update()
    {
        fireSfxTimer -= Time.deltaTime;
        iceSfxTimer -= Time.deltaTime;
        lightningSfxTimer -= Time.deltaTime;

        HandleMovementInput();
        HandleAutoFire();
        HandleSpecials();
        HandleMovementLoop();
        UpdateSpecialUI();
    }

    void FixedUpdate()
    {
        // classic top-down movement
        float speed = moveSpeed;
        if (upgradeManager != null)
            speed *= upgradeManager.moveSpeedMultiplier;

        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void HandleMovementLoop()
    {
        if (moveLoopSource == null)
            return;

        AudioClip wantedClip = ComboReady() ? empoweredMoveLoopSfx : moveLoopSfx;
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (!isMoving || wantedClip == null || isDead)
        {
            if (moveLoopSource.isPlaying)
                moveLoopSource.Stop();
            return;
        }

        if (moveLoopSource.clip != wantedClip)
        {
            moveLoopSource.clip = wantedClip;

            float speed = rb.velocity.magnitude;
            float maxSpeed = moveSpeed;
            float t = Mathf.Clamp01(speed / maxSpeed);
            moveLoopSource.pitch = Mathf.Lerp(1.3f, 3f, t);

            moveLoopSource.volume = moveLoopVolume;
            moveLoopSource.Play();
        }
        else if (!moveLoopSource.isPlaying)
        {
            moveLoopSource.volume = moveLoopVolume;
            moveLoopSource.Play();
        }
    }

    bool ComboReady()
    {
        return fireRunes >= 5 && iceRunes >= 5 && lightningRunes >= 5;
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

        ProjectileBase pb = proj.GetComponent<ProjectileBase>();
        if (pb != null)
        {
            pb.SetDirection(dir);

            if (upgradeManager != null)
            {
                // scale projectile speed
                pb.speed *= upgradeManager.projectileSpeedMultiplier;

                // scale projectile damage
                pb.damage = Mathf.RoundToInt(pb.damage * upgradeManager.damageMultiplier);
            }
        }

        // play sfx but don't spam every single shot
        if (audioSrc != null)
        {
            if (prefab == fireProjectile && fireShootSfx != null && fireSfxTimer <= 0f)
            {
                PlayRandomizedSfx(fireShootSfx, fireShootVolume);
                fireSfxTimer = 0.15f; // min gap between fire sounds
            }

            if (prefab == iceProjectile && iceShootSfx != null && iceSfxTimer <= 0f)
            {
                PlayRandomizedSfx(iceShootSfx, iceShootVolume);
                iceSfxTimer = 0.2f;
            }

            if (prefab == lightningProjectile && lightningShootSfx != null && lightningSfxTimer <= 0f)
            {
                PlayRandomizedSfx(lightningShootSfx, lightningShootVolume);
                lightningSfxTimer = 0.2f;
            }
        }
    }

    void PlayRandomizedSfx(AudioClip clip, float volume)
    {
        if (audioSrc == null || clip == null)
            return;

        float oldPitch = audioSrc.pitch;
        audioSrc.pitch = Random.Range(minPitch, maxPitch);
        audioSrc.PlayOneShot(clip, volume);
        audioSrc.pitch = oldPitch;
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
        if (upgradeMenu != null && upgradeMenu.IsOpen())
            return;

        Vector2 mouseDir = GetMouseDirection();

        // left click = fire special
        if (Input.GetMouseButtonDown(0) && fireRunes >= 5)
        {
            GameObject special = Instantiate(fireSpecial, transform.position, Quaternion.identity);
            special.GetComponent<FireSpecial>()?.SetDirection(mouseDir);
            fireRunes -= 5;
            PlayRandomizedSfx(fireSpecialSfx, fireSpecialVolume);
            UpdateSpecialUI();
        }

        // right click = ice special
        if (Input.GetMouseButtonDown(1) && iceRunes >= 5)
        {
            GameObject special = Instantiate(iceSpecial, transform.position, Quaternion.identity);
            special.GetComponent<IceSpecial>()?.SetDirection(mouseDir);
            iceRunes -= 5;
            PlayRandomizedSfx(iceSpecialSfx, iceSpecialVolume);
            UpdateSpecialUI();
        }

        // Q = lightning special (aoe)
        if ((Input.GetKeyDown(KeyCode.Q)) && lightningRunes >= 5)
        {
            Instantiate(lightningSpecial, transform.position, Quaternion.identity);
            lightningRunes -= 5;
            PlayRandomizedSfx(lightningSpecialSfx, lightningSpecialVolume);
            UpdateSpecialUI();
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
            PlayRandomizedSfx(comboSpecialSfx, comboSpecialVolume);
            UpdateSpecialUI();
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

        UpdateSpecialUI();
    }

    void UpdateSpecialUI()
    {
        // fire special
        int fireStacks = fireRunes / 5;
        float fireProgress = Mathf.Clamp01(fireRunes / 5f);
        fireUI.UpdateUI(fireStacks > 0, fireProgress, fireStacks);

        // ice special
        int iceStacks = iceRunes / 5;
        float iceProgress = Mathf.Clamp01(iceRunes / 5f);
        iceUI.UpdateUI(iceStacks > 0, iceProgress, iceStacks);

        // lightning
        int lightningStacks = lightningRunes / 5;
        float lightningProgress = Mathf.Clamp01(lightningRunes / 5f);
        lightningUI.UpdateUI(lightningStacks > 0, lightningProgress, lightningStacks);

        // combo: uses MIN of all 3
        int comboStacks = Mathf.Min(fireRunes / 5, iceRunes / 5, lightningRunes / 5);
        float comboProgress = Mathf.Clamp01(
            Mathf.Min(fireRunes, iceRunes, lightningRunes) / 5f
        );
        comboUI.UpdateUI(comboStacks > 0, comboProgress, comboStacks);
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

        PlayRandomizedSfx(ouchSfx, 1f);
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
        if (isDead) return;
        isDead = true;

        if (moveLoopSource != null && moveLoopSource.isPlaying)
            moveLoopSource.Stop();

        if (GameManager.instance != null)
            GameManager.instance.GameOver();

        Debug.Log("player died");
        enabled = false;
    }
}
