using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI fireRuneText;
    public TextMeshProUGUI iceRuneText;
    public TextMeshProUGUI lightningRuneText;
    public Slider healthBar;
    public Image damageFlash;         // red overlay image

    [Header("xp ui")]
    public Slider xpBar;              // shows progress to next level
    public TextMeshProUGUI levelText; // shows current level

    private float elapsedTime = 0f;
    private float flashAlpha = 0f;
    private float flashFadeSpeed = 2f;

    public GameObject gameOverPanel;
    public TextMeshProUGUI survivalTimeText;

    public static UIManager instance;

    void Start()
    {
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Update()
    {
        // timer text
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // fade flash out over time
        if (damageFlash != null && flashAlpha > 0f)
        {
            flashAlpha -= Time.deltaTime * flashFadeSpeed;
            Color c = damageFlash.color;
            c.a = flashAlpha;
            damageFlash.color = c;
        }
    }

    public void UpdateHealth(float newValue)
    {
        if (healthBar != null)
            healthBar.value = newValue;  // no smoothing, just snap to real hp
    }

    public void TriggerDamageFlash()
    {
        if (damageFlash == null) return;

        flashAlpha = 0.5f;
        Color c = damageFlash.color;
        c.a = flashAlpha;
        damageFlash.color = c;
    }

    public void UpdateXPBar(int level, int currentXP, int xpToNext)
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNext;
            xpBar.value = currentXP;
        }

        if (levelText != null)
            levelText.text = $"lv {level}";
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (survivalTimeText != null)
            survivalTimeText.text = $"You survived {minutes:00}:{seconds:00}";

        gameOverPanel.SetActive(true);
    }
}
